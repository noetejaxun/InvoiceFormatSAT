using InvoiceFormatSAT.AddIns;
using InvoiceFormatSAT.Controllers.API.BanGuat;
using InvoiceFormatSAT.Controllers.API.SAT;
using InvoiceFormatSAT.Models;
using InvoiceFormatSAT.Models.ExchangeRate;
using InvoiceFormatSAT.Models.Interfaces.InterfacesExchangeRate;
using InvoiceFormatSAT.Models.satApi;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace InvoiceFormatSAT.Controllers
{
    class InvoiceController
    {
        public async Task<Invoice> getDTE(HttpRequest req)
        {
            satAPI api = new satAPI();
            Invoice invoice = new Invoice(new InvoiceDocument());

            try
            {
                XmlQuery query = JsonConvert.DeserializeObject<XmlQuery>( await new StreamReader(req.Body).ReadToEndAsync() );

                ResponseValidation responseValidation = await api.getValidationFEL( query );

                if (responseValidation.detalle != null)
                {
                    invoice.DTEExists = responseValidation.detalle[0].existeDte;
                }

                if (invoice.DTEExists)
                {
                    invoice = await new InvoiceController().getInvoiceDocument(
                        Function.getConvertedXML((string)((JArray)JObject.Parse(
                            await api.getXmlFEL(responseValidation))["detalle"])[0]), responseValidation, query);

                    invoice.DTEExists = responseValidation.detalle[0].existeDte;
                }
                else
                {
                    invoice.DTE.HasError = true;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                invoice.DTE.HasError = true;
            }

            return invoice;
        }

        public async Task<Invoice> getInvoiceDocument(dynamic json, ResponseValidation responseValidation, XmlQuery query)
        {
            InvoiceDocument invoiceDocument = new InvoiceDocument();

            try
            {
                dynamic DTE             = Function.getDynamicObjectByXMLPath( json, new string[] { "dte:GTDocumento", "dte:SAT", "dte:DTE"}, 0);
                dynamic certificacion   = Function.getDynamicObject(DTE, "dte:Certificacion");
                dynamic DatosEmision    = Function.getDynamicObject(DTE, "dte:DatosEmision");
                dynamic issuer          = Function.getDynamicObject(DatosEmision, "dte:Emisor");
                dynamic receiver        = Function.getDynamicObject(DatosEmision, "dte:Receptor");
                dynamic generalData     = Function.getDynamicObject(DatosEmision, "dte:DatosGenerales");
                dynamic totales         = Function.getDynamicObject(DatosEmision, "dte:Totales");
                dynamic complementos    = Function.getDynamicObject(DatosEmision, "dte:Complementos");
                dynamic issuerAddress   = Function.getDynamicObjectByXMLPath(DatosEmision, new string[] { "dte:Emisor", "dte:DireccionEmisor" }, 0);
                dynamic receiverAddress = Function.getDynamicObjectByXMLPath(DatosEmision, new string[] { "dte:Receptor", "dte:DireccionReceptor" }, 0);
                dynamic items           = Function.getDynamicObjectByXMLPath(DatosEmision, new string[] { "dte:Items", "dte:Item" }, 0);
                dynamic phrases         = Function.getDynamicObjectByXMLPath(DatosEmision, new string[] { "dte:Frases", "dte:Frase" }, 0);

                if (phrases.GetType() == typeof(JObject))
                {
                    JArray newArray = new JArray();
                    newArray.Add(phrases);
                    phrases = newArray;
                }

                if (items.GetType() == typeof(JObject)) 
                {
                    JArray newArray = new JArray();
                    newArray.Add(items);
                    items = newArray;
                }

                IExchangeRateController exchange = new ExchangeRateController();

                DateTime documentDate           = Function.getDateTimeValue(generalData, "@FechaHoraEmision");
                IExchangeRate exchangeRate = null;

                if (query.moneda == "USD")
                {
                    exchangeRate = await exchange.getExchangeRate(documentDate, documentDate);
                }

                if (exchangeRate == null)
                {
                    exchangeRate = new ExchangeRate(0, DateTime.Now, 1, 1);
                }

                Issuer issuerInfo               = getIssuer(issuer, issuerAddress);
                Receiver receiverInfo           = getReceiver(receiver, receiverAddress);
                List<Phrases> Phrases           = getPhrases(phrases);
                List<Item> Items                = getItems(items, exchangeRate);
                List<TaxTotal> taxTotals        = getTotals(totales, exchangeRate);
                double grandTotal               = Function.getDoubleValue(totales, "dte:GranTotal") / exchangeRate.PurchaseValue;
                Total total                     = new Total(grandTotal, taxTotals);
                Certification certification     = getCertification(certificacion);
                List<Complement> complements    = new List<Complement>();

                invoiceDocument.AuthorizationID = responseValidation.detalle[0].numeroUUID;
                invoiceDocument.Serie           = responseValidation.detalle[0].serie;
                invoiceDocument.Number          = responseValidation.detalle[0].numeroDocumento;
                invoiceDocument.State           = responseValidation.detalle[0].estado;
                invoiceDocument.Canceled        = responseValidation.detalle[0].fechaHoraAnulacion == "" ? false : true;
                invoiceDocument.CanceledDate    = responseValidation.detalle[0].fechaHoraAnulacion;
                invoiceDocument.Type            = Function.getStringValue(generalData, "@Tipo");
                invoiceDocument.OriginalCurrency = Function.getStringValue(generalData, "@CodigoMoneda");
                invoiceDocument.Currency        = query.moneda == "USD" ? "USD" : invoiceDocument.OriginalCurrency;
                invoiceDocument.DocumentDate    = documentDate;
                invoiceDocument.Summary         = getSummary(Items);
                invoiceDocument.oilAmount       = getAmount(Items) / exchangeRate.PurchaseValue;
                invoiceDocument.Issuer          = issuerInfo;
                invoiceDocument.Receiver        = receiverInfo;
                invoiceDocument.Phrases         = Phrases;
                invoiceDocument.Items           = Items;
                invoiceDocument.Total           = total;
                invoiceDocument.Certification   = certification;
                                   
                if (invoiceDocument.Type == "NCRE")
                {
                    complements = getComplements(complementos);
                }

                invoiceDocument.Complements = complements;

                invoiceDocument.HasError = false;

                return new Invoice(invoiceDocument);
            }
            catch (Exception)            
            {
                invoiceDocument.HasError = true;
            }
            return new Invoice(invoiceDocument);

        }

        private List<Complement> getComplements(dynamic complements)
        {
            List<Complement> complementsList = new List<Complement>();

            foreach (dynamic complement in complements) {
                if (complement.Name == "dte:Complemento")
                {
                    dynamic complementRow = complement.Value;
                    if (complementRow != null)
                    {
                        dynamic noteReference = complementRow["cno:ReferenciasNota"];
                        if (noteReference != null)
                        {
                            try {
                                string documentDate     = Function.getStringValue(noteReference, "@FechaEmisionDocumentoOrigen");
                                string[] splitedDate    = documentDate.Split("-");
                                DateTime formatedDate   = DateTime.Now;

                                if (splitedDate.Length == 3)
                                {
                                    int year    = int.Parse(splitedDate[0]);
                                    int month   = int.Parse(splitedDate[1]);
                                    int day     = int.Parse(splitedDate[2]);

                                    formatedDate = new DateTime(year, month, day);
                                }

                                complementsList.Add(new Complement( Function.getStringValue(complementRow, "@IDComplemento"),
                                                                    Function.getStringValue(complementRow, "@NombreComplemento"),
                                                                    Function.getStringValue(complementRow, "@URIComplemento"),
                                                                    formatedDate,
                                                                    Function.getStringValue(noteReference, "@MotivoAjuste"),
                                                                    Function.getStringValue(noteReference, "@NumeroAutorizacionDocumentoOrigen") ));
                            } catch (Exception) {

                            }

                        }
                    }
                }
            }

            return complementsList;
        }

        private Issuer getIssuer(dynamic issuer, dynamic address)
        {

            return new Issuer( Function.getStringValue(issuer,  "@CodigoEstablecimiento"),
                               Function.getStringValue(issuer,  "@NITEmisor"),
                               Function.getStringValue(issuer,  "@NombreComercial"),
                               Function.getStringValue(issuer,  "@NombreEmisor"),
                               Function.getStringValue(issuer,  "@CorreoEmisor"),
                               Function.getStringValue(issuer,  "@AfiliacionIVA"),
                               Function.getStringValue(address, "dte:Direccion"),
                               Function.getStringValue(address, "dte:CodigoPostal"),
                               Function.getStringValue(address, "dte:Municipio"),
                               Function.getStringValue(address, "dte:Departamento"),
                               Function.getStringValue(address, "dte:Pais") );
        }

        private Receiver getReceiver(dynamic receiver, dynamic address)
        {

            return new Receiver( Function.getStringValue(receiver, "@IDReceptor"),
                                 Function.getStringValue(receiver, "@NombreReceptor"),
                                 Function.getStringValue(receiver, "@CorreoReceptor"),
                                 Function.getStringValue(address,  "dte:Direccion"),
                                 Function.getStringValue(address,  "dte:CodigoPostal"),
                                 Function.getStringValue(address,  "dte:Municipio"),
                                 Function.getStringValue(address,  "dte:Departamento"),
                                 Function.getStringValue(address,  "dte:Pais"));;;
        }

        private List<Phrases> getPhrases(dynamic phrases)
        {
            List<Phrases> listPhrases = new List<Phrases>();

            foreach (dynamic phrase in phrases)
            {
                string code = Function.getStringValue(phrase, "@CodigoEscenario");
                string type = Function.getStringValue(phrase, "@TipoFrase");

                if (code != null && type != null) {
                    listPhrases.Add(new Phrases(code, type));
                }

            }

            return listPhrases;
        }

        private List<Tax> getTaxes(dynamic item, IExchangeRate exchangeRate)
        {
            List<Tax> Taxes = new List<Tax>();

            dynamic taxes = Function.getDynamicObject(item, "dte:Impuestos");

            if (taxes == null)
            {
                return Taxes;
            }

            if (taxes.GetType() == typeof(JObject))
            {
                dynamic taxList = Function.getDynamicObject(taxes, "dte:Impuesto");

                try
                {
                    if (taxList != null)
                    {
                        if (taxList.GetType() == typeof(JObject))
                        {
                            int code                        = Function.getIntValue(taxList,    "dte:CodigoUnidadGravable");
                            string name                     = Function.getStringValue(taxList, "dte:NombreCorto");
                            double amount                   = Function.getDoubleValue(taxList, "dte:MontoGravable") / exchangeRate.PurchaseValue;
                            double taxAmount                = Function.getDoubleValue(taxList, "dte:MontoImpuesto") / exchangeRate.PurchaseValue;
                            double gravableUnitAmount       = Function.getDoubleValue(taxList, "dte:CantidadUnidadesGravables") / exchangeRate.PurchaseValue;

                            Taxes.Add(new Tax(name, code, amount, taxAmount, gravableUnitAmount));
                        }
                        else
                        {
                            foreach (var tax in taxList)
                            {
                                int code                        = Function.getIntValue(tax,    "dte:CodigoUnidadGravable");
                                string name                     = Function.getStringValue(tax, "dte:NombreCorto");
                                double amount                   = Function.getDoubleValue(tax, "dte:MontoGravable") / exchangeRate.PurchaseValue;
                                double taxAmount                = Function.getDoubleValue(tax, "dte:MontoImpuesto") / exchangeRate.PurchaseValue;
                                double gravableUnitAmount       = Function.getDoubleValue(tax, "dte:CantidadUnidadesGravables") / exchangeRate.PurchaseValue;

                                Taxes.Add(new Tax(name, code, amount, taxAmount, gravableUnitAmount));
                            }
                        }
                        
                    }
                    return Taxes;
                }
                catch (Exception)
                {
                    return Taxes;
                }

            } else
            {

                try
                {
                    if (taxes != null)
                    {
                        foreach (var tax in taxes)
                        {
                            int code                    = Function.getIntValue(tax,    "dte:CodigoUnidadGravable");
                            string name                 = Function.getStringValue(tax, "dte:NombreCorto");
                            double amount               = Function.getDoubleValue(tax, "dte:MontoGravable") / exchangeRate.PurchaseValue;
                            double taxAmount            = Function.getDoubleValue(tax, "dte:MontoImpuesto") / exchangeRate.PurchaseValue;
                            double gravableUnitAmount   = Function.getDoubleValue(tax, "dte:CantidadUnidadesGravables") / exchangeRate.PurchaseValue;

                            Taxes.Add(new Tax(name, code, amount, taxAmount, gravableUnitAmount));
                        }
                    }
                    return Taxes;
                }
                catch (Exception)
                {
                    return Taxes;
                }
            }
            
        }

        private string getSummary(List<Item> Items)
        {
            int count = 0;
            string summary = "";
            foreach (Item item in Items)
            {
                count++;
                if (count <= 3)
                {
                    summary = summary + item.Type + " - " + item.Description + "\n";
                }
            }

            return summary;
        }

        private double getAmount(List<Item> Items)
        {
            double amount = 0;

            try
            {
                Item taxTotal = Items.Find(x => x.Taxes.Find(y => y.Name == "PETROLEO") != null);
                if (taxTotal != null)
                {
                    amount = taxTotal.Amount;
                }
            }
            catch (Exception)
            {
                amount = 0;
            }

            return amount;
        }

        private List<Item> getItems(dynamic items, IExchangeRate exchangeRate)
        {
            List<Item> listOfItems = new List<Item>();

            try
            {
                foreach (dynamic item in items)
                {
                    int line                = Function.getIntValue(item,    "@NumeroLinea");
                    string type             = Function.getStringValue(item, "@BienOServicio");
                    double amount           = Function.getDoubleValue(item, "dte:Cantidad");
                    string unitOfMeasure    = Function.getStringValue(item, "dte:UnidadMedida");
                    string description      = Function.getStringValue(item, "dte:Descripcion");
                    double unitPrice        = Function.getDoubleValue(item, "dte:PrecioUnitario");
                    double price            = Function.getDoubleValue(item, "dte:Precio");
                    double discount         = Function.getDoubleValue(item, "dte:Descuento");
                    double total            = Function.getDoubleValue(item, "dte:Total");

                    List<Tax> taxes = getTaxes(item, exchangeRate);

                    listOfItems.Add(new Item( type,
                                              line,
                                              amount,
                                              unitOfMeasure,
                                              description,
                                              unitPrice / exchangeRate.PurchaseValue,
                                              price / exchangeRate.PurchaseValue,
                                              discount / exchangeRate.PurchaseValue,
                                              total / exchangeRate.PurchaseValue,
                                              taxes ));
                }

                return listOfItems;
            }
            catch (Exception)
            {
                return listOfItems;
            }

        }

        private List<TaxTotal> getTotals(dynamic totals, IExchangeRate exchangeRate)
        {
            List<TaxTotal> taxTotals = new List<TaxTotal>();

            try
            {
                dynamic totalTaxes = Function.getDynamicObject(totals, "dte:TotalImpuestos");

                if (totalTaxes != null)
                {
                    if (totalTaxes.GetType() == typeof(JObject))
                    {
                        dynamic taxTotal = Function.getDynamicObject(totalTaxes, "dte:TotalImpuesto");

                        if (taxTotal.GetType() == typeof(JObject))
                        {
                            string name   = Function.getStringValue(taxTotal, "@NombreCorto");
                            double amount = Function.getDoubleValue(taxTotal, "@TotalMontoImpuesto") / exchangeRate.PurchaseValue;

                            taxTotals.Add(new TaxTotal(name, amount));
                        } else
                        {
                            foreach (var tax in taxTotal)
                            {
                                string name   = Function.getStringValue(tax, "@NombreCorto");
                                double amount = Function.getDoubleValue(tax, "@TotalMontoImpuesto") / exchangeRate.PurchaseValue;

                                taxTotals.Add(new TaxTotal(name, amount));
                            }
                        }

                    }
                    else
                    {
                        foreach (var tax in totalTaxes)
                        {
                            string name   = Function.getStringValue(tax.Value, "@NombreCorto");
                            double amount = Function.getDoubleValue(tax.Value, "@TotalMontoImpuesto") / exchangeRate.PurchaseValue;

                            taxTotals.Add(new TaxTotal(name, amount));
                        }
                    }

                }

                return taxTotals;
            }
            catch (Exception)
            {
                return taxTotals;
            }

        }

        private Certification getCertification(dynamic certification)
        {
            Certification newCertification = new Certification();

            try
            {
                dynamic authorization = Function.getDynamicObject(certification, "dte:NumeroAutorizacion");

                if (authorization != null)
                {
                    newCertification.CertifierNIT       = Function.getStringValue(certification,   "dte:NITCertificador");
                    newCertification.CertifierName      = Function.getStringValue(certification,   "dte:NombreCertificador");
                    newCertification.CertificationDate  = Function.getDateTimeValue(certification, "dte:FechaHoraCertificacion");

                    dynamic values = JsonConvert.DeserializeObject( "{" +
                                                                        "'#text': 'AuthorizationID'," +
                                                                        "'@Serie': 'Serie'," +
                                                                        "'@Numero': 'Number'" +
                                                                    "}" );

                    foreach (dynamic auth in authorization)
                    {
                        var propInfo = newCertification.GetType().GetProperty( Function.getStringValue(values, auth.Name) );

                        if (propInfo.GetMethod.ReturnType == typeof(string))
                        {
                            propInfo.SetValue( newCertification, (string)auth.Value );
                        }
                        
                    }
                }

                return newCertification;
            }
            catch (Exception)
            {
                return newCertification;
            }

        }
    }
}
