using System;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Xml.Serialization;
using System.Threading.Tasks;

namespace USSD.Model
{
    [XmlRoot(ElementName="send-sms-status")]
    public class ModelXML
    {
        /// Class Answer to USSD Command  http://192.168.112.201/default/en_US/send_status.xml?u=admin&p=password
		[XmlElement(ElementName="id1")]
		public string Id1 { get; set; }
		[XmlElement(ElementName="status1")]
		public string Status1 { get; set; }
		[XmlElement(ElementName="error1")]
		public string Error1 { get; set; }
		[XmlElement(ElementName="id2")]
		public string Id2 { get; set; }
		[XmlElement(ElementName="status2")]
		public string Status2 { get; set; }
		[XmlElement(ElementName="error2")]
		public string Error2 { get; set; }
		[XmlElement(ElementName="id3")]
		public string Id3 { get; set; }
		[XmlElement(ElementName="status3")]
		public string Status3 { get; set; }
		[XmlElement(ElementName="error3")]
		public string Error3 { get; set; }
		[XmlElement(ElementName="id4")]
		public string Id4 { get; set; }
		[XmlElement(ElementName="status4")]
		public string Status4 { get; set; }
		[XmlElement(ElementName="error4")]
		public string Error4 { get; set; }
		[XmlElement(ElementName="id5")]
		public string Id5 { get; set; }
		[XmlElement(ElementName="status5")]
		public string Status5 { get; set; }
		[XmlElement(ElementName="error5")]
		public string Error5 { get; set; }
		[XmlElement(ElementName="id6")]
		public string Id6 { get; set; }
		[XmlElement(ElementName="status6")]
		public string Status6 { get; set; }
		[XmlElement(ElementName="error6")]
		public string Error6 { get; set; }
		[XmlElement(ElementName="id7")]
		public string Id7 { get; set; }
		[XmlElement(ElementName="status7")]
		public string Status7 { get; set; }
		[XmlElement(ElementName="error7")]
		public string Error7 { get; set; }
		[XmlElement(ElementName="id8")]
		public string Id8 { get; set; }
		[XmlElement(ElementName="status8")]
		public string Status8 { get; set; }
		[XmlElement(ElementName="error8")]
		public string Error8 { get; set; }
		[XmlElement(ElementName="id9")]
		public string Id9 { get; set; }
		[XmlElement(ElementName="status9")]
		public string Status9 { get; set; }
		[XmlElement(ElementName="error9")]
		public string Error9 { get; set; }
		[XmlElement(ElementName="id10")]
		public string Id10 { get; set; }
		[XmlElement(ElementName="status10")]
		public string Status10 { get; set; }
		[XmlElement(ElementName="error10")]
		public string Error10 { get; set; }
		[XmlElement(ElementName="id11")]
		public string Id11 { get; set; }
		[XmlElement(ElementName="status11")]
		public string Status11 { get; set; }
		[XmlElement(ElementName="error11")]
		public string Error11 { get; set; }
		[XmlElement(ElementName="id12")]
		public string Id12 { get; set; }
		[XmlElement(ElementName="status12")]
		public string Status12 { get; set; }
		[XmlElement(ElementName="error12")]
		public string Error12 { get; set; }
		[XmlElement(ElementName="id13")]
		public string Id13 { get; set; }
		[XmlElement(ElementName="status13")]
		public string Status13 { get; set; }
		[XmlElement(ElementName="error13")]
		public string Error13 { get; set; }
		[XmlElement(ElementName="id14")]
		public string Id14 { get; set; }
		[XmlElement(ElementName="status14")]
		public string Status14 { get; set; }
		[XmlElement(ElementName="error14")]
		public string Error14 { get; set; }
		[XmlElement(ElementName="id15")]
		public string Id15 { get; set; }
		[XmlElement(ElementName="status15")]
		public string Status15 { get; set; }
		[XmlElement(ElementName="error15")]
		public string Error15 { get; set; }
		[XmlElement(ElementName="id16")]
		public string Id16 { get; set; }
		[XmlElement(ElementName="status16")]
		public string Status16 { get; set; }
		[XmlElement(ElementName="error16")]
		public string Error16 { get; set; }
		[XmlElement(ElementName="id17")]
		public string Id17 { get; set; }
		[XmlElement(ElementName="status17")]
		public string Status17 { get; set; }
		[XmlElement(ElementName="error17")]
		public string Error17 { get; set; }
		[XmlElement(ElementName="id18")]
		public string Id18 { get; set; }
		[XmlElement(ElementName="status18")]
		public string Status18 { get; set; }
		[XmlElement(ElementName="error18")]
		public string Error18 { get; set; }
		[XmlElement(ElementName="id19")]
		public string Id19 { get; set; }
		[XmlElement(ElementName="status19")]
		public string Status19 { get; set; }
		[XmlElement(ElementName="error19")]
		public string Error19 { get; set; }
		[XmlElement(ElementName="id20")]
		public string Id20 { get; set; }
		[XmlElement(ElementName="status20")]
		public string Status20 { get; set; }
		[XmlElement(ElementName="error20")]
		public string Error20 { get; set; }
		[XmlElement(ElementName="id21")]
		public string Id21 { get; set; }
		[XmlElement(ElementName="status21")]
		public string Status21 { get; set; }
		[XmlElement(ElementName="error21")]
		public string Error21 { get; set; }
		[XmlElement(ElementName="id22")]
		public string Id22 { get; set; }
		[XmlElement(ElementName="status22")]
		public string Status22 { get; set; }
		[XmlElement(ElementName="error22")]
		public string Error22 { get; set; }
		[XmlElement(ElementName="id23")]
		public string Id23 { get; set; }
		[XmlElement(ElementName="status23")]
		public string Status23 { get; set; }
		[XmlElement(ElementName="error23")]
		public string Error23 { get; set; }
		[XmlElement(ElementName="id24")]
		public string Id24 { get; set; }
		[XmlElement(ElementName="status24")]
		public string Status24 { get; set; }
		[XmlElement(ElementName="error24")]
		public string Error24 { get; set; }
		[XmlElement(ElementName="id25")]
		public string Id25 { get; set; }
		[XmlElement(ElementName="status25")]
		public string Status25 { get; set; }
		[XmlElement(ElementName="error25")]
		public string Error25 { get; set; }
		[XmlElement(ElementName="id26")]
		public string Id26 { get; set; }
		[XmlElement(ElementName="status26")]
		public string Status26 { get; set; }
		[XmlElement(ElementName="error26")]
		public string Error26 { get; set; }
		[XmlElement(ElementName="id27")]
		public string Id27 { get; set; }
		[XmlElement(ElementName="status27")]
		public string Status27 { get; set; }
		[XmlElement(ElementName="error27")]
		public string Error27 { get; set; }
		[XmlElement(ElementName="id28")]
		public string Id28 { get; set; }
		[XmlElement(ElementName="status28")]
		public string Status28 { get; set; }
		[XmlElement(ElementName="error28")]
		public string Error28 { get; set; }
		[XmlElement(ElementName="id29")]
		public string Id29 { get; set; }
		[XmlElement(ElementName="status29")]
		public string Status29 { get; set; }
		[XmlElement(ElementName="error29")]
		public string Error29 { get; set; }
		[XmlElement(ElementName="id30")]
		public string Id30 { get; set; }
		[XmlElement(ElementName="status30")]
		public string Status30 { get; set; }
		[XmlElement(ElementName="error30")]
		public string Error30 { get; set; }
		[XmlElement(ElementName="id31")]
		public string Id31 { get; set; }
		[XmlElement(ElementName="status31")]
		public string Status31 { get; set; }
		[XmlElement(ElementName="error31")]
		public string Error31 { get; set; }
		[XmlElement(ElementName="id32")]
		public string Id32 { get; set; }
		[XmlElement(ElementName="status32")]
		public string Status32 { get; set; }
		[XmlElement(ElementName="error32")]
		public string Error32 { get; set; }
    }

}