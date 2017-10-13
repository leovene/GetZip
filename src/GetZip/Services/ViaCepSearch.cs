﻿using GetZip.Enums;
using GetZip.Http;
using GetZip.ValueObject;
using HelperConversion;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GetZip.Services
{
    public class ViaCepSearch : ISearch
    {
        #region constants
        private const string URL = "https://viacep.com.br/ws";
        private const string DOMAIN = "https://viacep.com.br";
        #endregion

        public async Task<bool> IsOnline()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(DOMAIN);
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<Address> GetAddress(string zipCode)
        {
            try
            {
                var data = $"{URL}/{zipCode.GetOnlyNumbers()}/xml";

                string result = await RequestSearch.GetResponse(URL, data, MethodOption.GET);
                if (result != null)
                {
                    var doc = XDocument.Parse(result);
                    var element = doc.Descendants("xmlcep").FirstOrDefault();
                    var address = new Address(element.Element("cep").Value.GetOnlyNumbers(), element.Element("logradouro").Value.Split(" ")[0].Trim(),
                        element.Element("logradouro").Value, element.Element("complemento").Value, element.Element("bairro").Value,
                        element.Element("localidade").Value, element.Element("uf").Value, "");
                    return address;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
