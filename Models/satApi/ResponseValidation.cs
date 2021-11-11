using InvoiceFormatSAT.Models.Interfaces.InterfacesSatApi;
using System.Collections.Generic;

namespace InvoiceFormatSAT.Models.satApi
{
    public class ResponseValidation : IResponseValidation
    {
        public int estadoHttp { get; set; }
        public int tamanioDetalle { get; set; }
        public List<Detail> detalle { get; set; }
    }
}
