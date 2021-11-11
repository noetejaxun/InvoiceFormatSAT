using InvoiceFormatSAT.Models.Interfaces.InterfacesSatApi;
using System;

namespace InvoiceFormatSAT.Models.satApi
{
    public class Detail : IDetail
    {
        public string nombreEmisor { get; set; }
        public string numeroUUID { get; set; }
        public string nitEmisor { get; set; }
        public string idEstablecimiento { get; set; }
        public string faceId { get; set; }
        public string nombreEstablecimiento { get; set; }
        public DateTime fechaEmision { get; set; }
        public string nombreReceptor { get; set; }
        public string estado { get; set; }
        public string idReceptor { get; set; }
        public double granTotal { get; set; }
        public string serie { get; set; }
        public string moneda { get; set; }
        public string numeroDocumento { get; set; }
        public bool existeDte { get; set; }
        public DateTime fechaHoraConsulta { get; set; }
        public bool exportacion { get; set; }
        public string fechaHoraAnulacion { get; set; }
        public string TIPO { get; set; }
    }
}
