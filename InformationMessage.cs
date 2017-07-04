using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSharp
{
    /// <summary>
    /// Holds informational message provided by DB
    /// </summary>
    public class InformationMessage
    {
        public InformationMessage(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid))
                throw new ArgumentException("UID can not be null or empty");
            Uid = uid;
        }

        /// <summary>
        /// id
        /// </summary>
        public string Uid { get; protected set; }

        /// <summary>
        /// t
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// c
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// ts
        /// </summary>
        public DateTime? Time { get; set; }

        /// <summary>
        /// Incomplete mapping of descriptions for the different message types
        /// </summary>
        /// <returns>Human readable version of the Type</returns>
        public string GetHumanString()
        {
            switch (Value) {
                case "2": return "Polizeiliche Ermittlung";
                case "3": return "Feuerwehreinsatz neben der Strecke";
                case "5": return "Ärztliche Versorgung eines Fahrgastes";
                case "6": return "Betätigen der Notbremse";
                case "7": return "Personen im Gleis";
                case "8": return "Notarzteinsatz am Gleis";
                case "9": return "Streikauswirkungen";
                case "10": return "Ausgebrochene Tiere im Gleis";
                case "11": return "Unwetter";
                case "13": return "Pass- und Zollkontrolle";
                case "15": return "Beeinträchtigung durch Vandalismus";
                case "16": return "Entschärfung einer Fliegerbombe";
                case "17": return "Beschädigung einer Brücke";
                case "18": return "Umgestürzter Baum im Gleis";
                case "19": return "Unfall an einem Bahnübergang";
                case "20": return "Tiere im Gleis";
                case "21": return "Warten auf weitere Reisende";
                case "22": return "Witterungsbedingte Störung";
                case "23": return "Feuerwehreinsatz auf Bahngelände";
                case "24": return "Verspätung aus dem Ausland";
                case "25": return "Warten auf verspätete Zugteile";
                case "28": return "Gegenstände im Gleis";
                case "31": return "Bauarbeiten";
                case "32": return "Verzögerung beim Ein-/Ausstieg";
                case "33": return "Oberleitungsstörung";
                case "34": return "Signalstörung";
                case "35": return "Streckensperrung";
                case "36": return "Technische Störung am Zug";
                case "38": return "Technische Störung an der Strecke";
                case "39": return "Anhängen von zusätzlichen Wagen";
                case "40": return "Stellwerksstörung/-ausfall";
                case "41": return "Störung an einem Bahnübergang";
                case "42": return "Außerplanmäßige Geschwindigkeitsbeschränkung";
                case "43": return "Verspätung eines vorausfahrenden Zuges";
                case "44": return "Warten auf einen entgegenkommenden Zug";
                case "45": return "Überholung durch anderen Zug";
                case "46": return "Warten auf freie Einfahrt";
                case "47": return "Verspätete Bereitstellung";
                case "48": return "Verspätung aus vorheriger Fahrt";
                case "55": return "Technische Störung an einem anderen Zug";           
	            case "56": return "Warten auf Fahrgäste aus einem Bus";
                case "57": return "Zusätzlicher Halt";
                case "58": return "Umleitung";                                          
	            case "59": return "Schnee und Eis";
                case "60": return "Reduzierte Geschwindigkeit wegen Sturm";
                case "61": return "Türstörung";
                case "62": return "Behobene technische Störung am Zug";
                case "63": return "Technische Untersuchung am Zug";
                case "64": return "Weichenstörung";
                case "65": return "Erdrutsch";
                case "70": return "Kein WLAN";
                case "71": return "WLAN in einzelnen Wagen nicht verfügbar";
                case "73": return "Mehrzweckabteil vorne";
                case "74": return "Mehrzweckabteil hinten";
                case "75": return "1. Klasse vorne";
                case "76": return "1. Klasse hinten";
                case "77": return "Ohne 1. Klasse";
                case "79": return "Ohne Mehrzweckabteil";
                case "80": return "Abweichende Wagenreihung";
                case "82": return "Mehrere Wagen fehlen";
                case "83": return "Fehlender Zugteil";
                case "84": return "Zug verkehrt richtig gereiht";
                case "85": return "Ein Wagen fehlt";
                case "86": return "Keine Reservierungsanzeige";
                case "87": return "Einzelne Wagen ohne Reservierungsanzeige";
                case "88": return "Keine Qualitätsmängel";
                case "89": return "Reservierungen sind wieder vorhanden";
                case "90": return "Kein Bordrestaurant/Bordbistro";
                case "91": return "Eingeschränkte Fahrradmitnahme";
                case "92": return "Klimaanlage in einzelnen Wagen ausgefallen";
                case "93": return "Fehlende oder gestörte behindertengerechte Einrichtung";
                case "94": return "Ersatzbewirtschaftung";
                case "95": return "Ohne behindertengerechtes WC";
                case "96": return "Der Zug ist stark überbesetzt";
                case "97": return "Der Zug ist überbesetzt";
                case "98": return "Sonstige Qualitätsmängel";
                case "99": return "Verzögerungen im Betriebsablauf";
                case "900": return "Anschlussbus wartet";
                default: return "Unbekannt";
            }
        }

        public override string ToString()
        {
            return GetHumanString();
        }

        public void MergeChanges(InformationMessage changes)
        {
            if (changes != null)
            {
                if(changes.Uid != this.Uid)
                    throw new ArgumentException("Cannot merge messages with different uids!");
                Type = changes.Type ?? Type;
                Value = changes.Value ?? Value;
                Time = changes.Time ?? Time;
            }
        }
    }
}
