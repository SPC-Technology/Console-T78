using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ_EInvoiceT78_Consumer.Base;
using RabbitMQ_EInvoiceT78_Consumer.Handle_Response;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_EInvoiceT78_Consumer
{
    class Program
    {
        static string queueName = "Q-0303430876-24C224BF797749F69A93BEEC0C5C0D10"; // Q trong thông itn kết nối
        static string URI = "amqp://U-0303430876-B9232524F7E6432982E5B8607E7FDB93:uFFBjZtHLHrG@14.225.17.176:5671"; // U- trong thông tin kết nối, mật khẩu
        static void Main(string[] args)
        {
            try
            {

                Config.RegisterServices();
                Config.RegisterDataSelectorServices();
                IList<string> hostsName = new List<string>();
                hostsName.Add("14.225.17.176");
                hostsName.Add("14.225.17.177");
                hostsName.Add("14.225.17.178");
                var factory = new ConnectionFactory()
                {
                    Uri = new Uri(URI),
                    Ssl = new SslOption()
                    {
                        Enabled = true,
                        ServerName = "14.225.17.176",
                        Version = SslProtocols.Tls12,
                        AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                                SslPolicyErrors.RemoteCertificateChainErrors
                    },
                    RequestedHeartbeat = TimeSpan.FromSeconds(60),
                    VirtualHost = "/",
                };
                var connection = factory.CreateConnection(hostsName);
                //var connection = factory.CreateConnection();
                using (var channel = connection.CreateModel())
                {
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);



                        var messageFeedBack = new Handle_Response.HandleResponse().GetResponse(message).Result;

                        if (messageFeedBack == TVAN_CONST.TAG_QUEUE.SUCCESS)
                        {
                            // Ack: thông báo đã xử lý message thành công và xóa khỏi queue
                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        }

                        //Nack: thông báo trả lại message cho queue với trường hợp xử lý lỗi hoặc muốn xử lý sau, mess sẽ push lại queue
                        if (messageFeedBack == TVAN_CONST.TAG_QUEUE.ERROR)
                        {
                            channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                        }



                        
                        Console.WriteLine("Consumer: " + message+"           ");
                    };

                    // autoAck: true : đọc xong sẽ xóa trên queue, false: vẫn giữ lại trên queue
                    channel.BasicConsume(queue: queueName,
                                         autoAck: false,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
                catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

        }
    }
}
