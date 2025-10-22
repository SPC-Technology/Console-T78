using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RabbitMQ_EInvoiceT78_Consumer.TimeStamp_Server
{
    public class TimeStamp
    {

        //Get timeStamp
        private static byte[] GetXmlHashByteStream(XmlDocument xmlDoc)
        {
            byte[] hash;
            XmlDsigC14NTransform transformer = new XmlDsigC14NTransform();
            transformer.LoadInput(xmlDoc);
            using (Stream stream = (Stream)transformer.GetOutput(typeof(Stream)))
            {
                SHA1 sha1 = SHA1.Create();
                hash = sha1.ComputeHash(stream);
                stream.Close();
            }
            return hash;
        }
        //string stampURI = "http://timestamp.digicert.com";
        //string stampURI = "http://time.certum.pl";
        private TimeStampResponse GetSignedHashFromTsa(byte[] hash)
        {
            TimeStampRequestGenerator reqGen = new TimeStampRequestGenerator();

            TimeStampRequest request = reqGen.Generate(
                        TspAlgorithms.Sha1,
                        hash,
                        BigInteger.ValueOf(100)
                    );
            byte[] reqData = request.GetEncoded();

            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create("http://zeitstempel.dfn.de");
            httpReq.Method = "POST";
            httpReq.ContentType = "application/timestamp-query";
            httpReq.ContentLength = reqData.Length;

            //Configure Timeout
            //httpReq.Timeout = 5000;
            //httpReq.ReadWriteTimeout = 32000;

            // Write the request content
            Stream reqStream = httpReq.GetRequestStream();
            reqStream.Write(reqData, 0, reqData.Length);
            reqStream.Close();

            HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse();
            // Read the response
            Stream respStream = new BufferedStream(httpResp.GetResponseStream());
            TimeStampResponse response = new TimeStampResponse(respStream);
            respStream.Close();
            return response;
        }

        internal string GetSignedHashFromTsa(XmlDocument xmlDxocument)
        {
            byte[] hash = GetXmlHashByteStream(xmlDxocument);
            TimeStampResponse timeStampResponse = GetSignedHashFromTsa(hash);
            byte[] signedEncodedByteStream = timeStampResponse.GetEncoded();
            return Convert.ToBase64String(signedEncodedByteStream);
        }

        internal DateTime GetTsaTimeFromSignedHash(string tsaSignedHashString)
        {
            byte[] bytes = Convert.FromBase64String(tsaSignedHashString);
            TimeStampResponse timeStampResponse = new TimeStampResponse(bytes);
            return timeStampResponse.TimeStampToken.TimeStampInfo.GenTime;
        }
    }
}
