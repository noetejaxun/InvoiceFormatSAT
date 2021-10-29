using InvoiceFormatSAT.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace InvoiceFormatSAT.Controllers
{
    class TaxFile
    {
        public TaxFile(UploadXML upload)
        {
            this.upload = upload;
        }
        private UploadXML upload;
        public TaxDocument getTaxDocument(dynamic json)
        {
            dynamic GTDocument      = upload.getDynamicObject(json, "dte:GTDocumento");
            dynamic SAT             = upload.getDynamicObject(GTDocument, "dte:SAT");
            dynamic DTE             = upload.getDynamicObject(SAT, "dte:DTE");
            dynamic DatosEmision    = upload.getDynamicObject(DTE, "dte:DatosEmision");
            dynamic issuer          = upload.getDynamicObject(DatosEmision, "dte:Emisor");
            dynamic issuerAddress   = upload.getDynamicObject(issuer, "dte:DireccionEmisor");
            dynamic receiver        = upload.getDynamicObject(DatosEmision, "dte:Receptor");
            dynamic receiverAddress = upload.getDynamicObject(receiver, "dte:DireccionReceptor");
            dynamic Phrase          = upload.getDynamicObject(DatosEmision, "dte:Frases");
            dynamic phrases         = upload.getDynamicObject(Phrase, "dte:Frase");
            if (phrases.GetType() == typeof(JObject))
            {
                JArray newArray = new JArray();
                newArray.Add(phrases);
                phrases = newArray;
            }
            dynamic Item            = upload.getDynamicObject(DatosEmision, "dte:Items");
            dynamic items           = upload.getDynamicObject(Item, "dte:Item");
            if (items.GetType() == typeof(JObject)) 
            {
                JArray newArray = new JArray();
                newArray.Add(items);
                items = newArray;
            }
            dynamic totales         = upload.getDynamicObject(DatosEmision, "dte:Totales");
            dynamic certificacion   = upload.getDynamicObject(DTE, "dte:Certificacion");
            dynamic generalData     = upload.getDynamicObject(DatosEmision, "dte:DatosGenerales");

            Issuer issuerInfo           = getIssuer(issuer, issuerAddress);
            Receiver receiverInfo       = getReceiver(receiver, receiverAddress);
            List<Phrases> Phrases       = getPhrases(phrases);
            List<Item> Items            = getItems(items);
            string summary              = getSummary(Items);
            double amount               = getAmount(Items);
            List<TaxTotal> taxTotals    = getTotals(totales);
            double grandTotal           = double.Parse(upload.getDynamicValue(totales, "dte:GranTotal"));
            Total total                 = new Total(grandTotal, taxTotals);
            Certification certification = getCertification(certificacion);

            TaxDocument taxDocument     = new TaxDocument();
            taxDocument.Currency        = (string)upload.getDynamicValue(generalData, "@CodigoMoneda");
            taxDocument.Type            = (string)upload.getDynamicValue(generalData, "@Tipo");
            taxDocument.DocumentDate    = (DateTime)generalData["@FechaHoraEmision"];
            taxDocument.Summary         = summary;
            taxDocument.Cantidad        = amount;
            taxDocument.Issuer          = issuerInfo;
            taxDocument.Receiver        = receiverInfo;
            taxDocument.Phrases         = Phrases;
            taxDocument.Items           = Items;
            taxDocument.Total           = total;
            taxDocument.Certification   = certification;

            return taxDocument;
        }

        public Issuer getIssuer(dynamic issuer, dynamic address)
        {

            return new Issuer( upload.getDynamicValue(issuer, "@CodigoEstablecimiento"),
                               upload.getDynamicValue(issuer, "@NITEmisor"),
                               upload.getDynamicValue(issuer, "@NombreComercial"),
                               upload.getDynamicValue(issuer, "@NombreEmisor"),
                               upload.getDynamicValue(issuer, "@CorreoEmisor"),
                               upload.getDynamicValue(issuer, "@AfiliacionIVA"),
                               upload.getDynamicValue(address, "dte:Direccion"),
                               upload.getDynamicValue(address, "dte:CodigoPostal"),
                               upload.getDynamicValue(address, "dte:Municipio"),
                               upload.getDynamicValue(address, "dte:Departamento"),
                               upload.getDynamicValue(address, "dte:Pais") );
        }
        public Receiver getReceiver(dynamic receiver, dynamic address)
        {

            return new Receiver( upload.getDynamicValue(receiver, "@IDReceptor"),
                                 upload.getDynamicValue(receiver, "@NombreReceptor"),
                                 upload.getDynamicValue(receiver, "@CorreoReceptor"),
                                 upload.getDynamicValue(address, "dte:Direccion"),
                                 upload.getDynamicValue(address, "dte:CodigoPostal"),
                                 upload.getDynamicValue(address, "dte:Municipio"),
                                 upload.getDynamicValue(address, "dte:Departamento"),
                                 upload.getDynamicValue(address, "dte:Pais"));;;
        }

        public List<Phrases> getPhrases(dynamic phrases)
        {
            List<Phrases> listPhrases = new List<Phrases>();

            foreach (dynamic phrase in phrases)
            {
                string code = upload.getDynamicValue(phrase, "@CodigoEscenario");
                string type = upload.getDynamicValue(phrase, "@TipoFrase");

                listPhrases.Add(new Phrases(code, type));
            }

            return listPhrases;
        }

        public List<Tax> getTaxes(dynamic item)
        {
            List<Tax> Taxes = new List<Tax>();

            dynamic taxes = upload.getDynamicObject(item, "dte:Impuestos");

            if (taxes.GetType() == typeof(JObject))
            {
                dynamic taxList = upload.getDynamicObject(taxes, "dte:Impuesto");

                try
                {
                    if (taxList != null)
                    {
                        if (taxList.GetType() == typeof(JObject))
                        {
                            string name = upload.getDynamicValue(taxList, "dte:NombreCorto");
                            int code = int.Parse(upload.getDynamicValue(taxList, "dte:CodigoUnidadGravable"));
                            string textAmount = upload.getDynamicValue(taxList, "dte:MontoGravable");
                            double amount = double.Parse(string.IsNullOrEmpty(textAmount) ? "0" : textAmount);
                            string textTaxtAmount = upload.getDynamicValue(taxList, "dte:MontoImpuesto");
                            double taxAmount = double.Parse(string.IsNullOrEmpty(textTaxtAmount) ? "0" : textTaxtAmount);
                            string textGravableUnitAmount = upload.getDynamicValue(taxList, "dte:CantidadUnidadesGravables");
                            double gravableUnitAmount = double.Parse(string.IsNullOrEmpty(textGravableUnitAmount) ? "0" : textGravableUnitAmount);

                            Taxes.Add(new Tax(name, code, amount, taxAmount, gravableUnitAmount));
                        }
                        else
                        {
                            foreach (var tax in taxList)
                            {
                                string name = upload.getDynamicValue(tax, "dte:NombreCorto");
                                int code = int.Parse(upload.getDynamicValue(tax, "dte:CodigoUnidadGravable"));
                                string textAmount = upload.getDynamicValue(tax, "dte:MontoGravable");
                                double amount = double.Parse(string.IsNullOrEmpty(textAmount) ? "0" : textAmount);
                                string textTaxtAmount = upload.getDynamicValue(tax, "dte:MontoImpuesto");
                                double taxAmount = double.Parse(string.IsNullOrEmpty(textTaxtAmount) ? "0" : textTaxtAmount);
                                string textGravableUnitAmount = upload.getDynamicValue(tax, "dte:CantidadUnidadesGravables");
                                double gravableUnitAmount = double.Parse(string.IsNullOrEmpty(textGravableUnitAmount) ? "0" : textGravableUnitAmount);

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
                            string name = upload.getDynamicValue(tax, "dte:NombreCorto");
                            int code = int.Parse(upload.getDynamicValue(tax, "dte:CodigoUnidadGravable"));
                            string textAmount = upload.getDynamicValue(tax, "dte:MontoGravable");
                            double amount = double.Parse(string.IsNullOrEmpty(textAmount) ? "0" : textAmount);
                            string textTaxtAmount = upload.getDynamicValue(tax, "dte:MontoImpuesto");
                            double taxAmount = double.Parse(string.IsNullOrEmpty(textTaxtAmount) ? "0" : textTaxtAmount);
                            string textGravableUnitAmount = upload.getDynamicValue(tax, "dte:CantidadUnidadesGravables");
                            double gravableUnitAmount = double.Parse(string.IsNullOrEmpty(textGravableUnitAmount) ? "0" : textGravableUnitAmount);

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

        public string getSummary(List<Item> Items)
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

        public double getAmount(List<Item> Items)
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

        public List<Item> getItems(dynamic items)
        {
            List<Item> listOfItems = new List<Item>();

            try
            {
                foreach (dynamic item in items)
                {
                    string type = upload.getDynamicValue(item, "@BienOServicio");
                    int line = int.Parse(upload.getDynamicValue(item, "@NumeroLinea"));
                    double amount = double.Parse( upload.getDynamicValue(item, "dte:Cantidad") );
                    string unitOfMeasure = upload.getDynamicValue(item, "dte:UnidadMedida");
                    string description = upload.getDynamicValue(item, "dte:Descripcion");
                    double unitPrice = double.Parse( upload.getDynamicValue(item, "dte:PrecioUnitario") );
                    double price = double.Parse( upload.getDynamicValue(item, "dte:Precio") );
                    double discount = double.Parse( upload.getDynamicValue(item, "dte:Descuento") );
                    double total = double.Parse( upload.getDynamicValue(item, "dte:Total") );
                    List<Tax> taxes = getTaxes(item);

                    listOfItems.Add(new Item( type,
                                              line,
                                              amount,
                                              unitOfMeasure,
                                              description,
                                              unitPrice,
                                              price,
                                              discount,
                                              total,
                                              taxes ));
                }

                return listOfItems;
            }
            catch (Exception)
            {
                return listOfItems;
            }

        }

        public List<TaxTotal> getTotals(dynamic totals)
        {
            List<TaxTotal> taxTotals = new List<TaxTotal>();

            try
            {
                dynamic totalTaxes = upload.getDynamicObject(totals, "dte:TotalImpuestos");

                if (totalTaxes != null)
                {
                    if (totalTaxes.GetType() == typeof(JObject))
                    {
                        dynamic taxTotal = upload.getDynamicObject(totalTaxes, "dte:TotalImpuesto");

                        if (taxTotal.GetType() == typeof(JObject))
                        {
                            string name = upload.getDynamicValue(taxTotal, "@NombreCorto");
                            string totalAmount = upload.getDynamicValue(taxTotal, "@TotalMontoImpuesto");
                            double amount = double.Parse(string.IsNullOrEmpty(totalAmount) ? "0" : totalAmount);

                            taxTotals.Add(new TaxTotal(name, amount));
                        } else
                        {
                            foreach (var tax in taxTotal)
                            {
                                string name = upload.getDynamicValue(tax, "@NombreCorto");
                                string totalAmount = upload.getDynamicValue(tax, "@TotalMontoImpuesto");
                                double amount = double.Parse(string.IsNullOrEmpty(totalAmount) ? "0" : totalAmount);

                                taxTotals.Add(new TaxTotal(name, amount));
                            }
                        }

                    }
                    else
                    {
                        foreach (var tax in totalTaxes)
                        {
                            string name = upload.getDynamicValue(tax.Value, "@NombreCorto");
                            string totalAmount = upload.getDynamicValue(tax.Value, "@TotalMontoImpuesto");
                            double amount = double.Parse(string.IsNullOrEmpty(totalAmount) ? "0" : totalAmount);

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

        public Certification getCertification(dynamic certification)
        {

            Certification newCertification = new Certification();

            try
            {
                newCertification.CertifierNIT = upload.getDynamicValue(certification, "dte:NITCertificador");
                newCertification.CertifierName = upload.getDynamicValue(certification, "dte:NombreCertificador");
                newCertification.CertificationDate = DateTime.Parse( upload.getDynamicValue(certification, "dte:FechaHoraCertificacion") );

                dynamic authorization = upload.getDynamicObject(certification, "dte:NumeroAutorizacion");

                foreach (var auth in authorization)
                {
                    if (auth.Name == "@Numero")
                    {
                        newCertification.Number = (string)auth.Value;
                    }
                    if (auth.Name == "@Serie")
                    {
                        newCertification.Serie = (string)auth.Value;
                    }
                    if (auth.Name == "#text")
                    {
                        newCertification.AuthorizationID = (string)auth.Value;
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
