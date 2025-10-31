using System.ComponentModel;

namespace Models.Common.Policy.Policy.Fire
{
    public class FireContentExcelData
    {
        public string SN { get; set; }
        public string Particulars { get; set; }
        public HouseholdContentType Type { get; set; }
        public decimal Rate { get; set; }
        public decimal Quantity { get; set; }
        public decimal Total { get; set; }
    }

    public enum HouseholdContentType
    {
        //[Description("भवन")]
        //Building = 'A',
        [Description("यन्त्र तथा उपकरण (उद्योग बीमाको हकमा प्रत्येक एक लाख रुपैया भन्दा बढी रकमको यन्त्र तथा उपकरण खरिद तथा " +
            "जडान मिती सहितको विवरण खुलाउने)")]
        Equipment = 'A',
        [Description("कच्चा पदार्थ")]
        RawMaterials = 'B',
        [Description("प्रक्रियाको क्रममा रहेको मौज्दात ( वोर्क इन प्रोग्रेस)")]
        WorkInProgress = 'C',
        [Description("तयारी बस्तु")]
        FinishedGoods = 'D',
        [Description("अर्ध तयारी बस्तु")]
        SemiFinishedGoods = 'E',
        [Description("फर्नीचर फिक्चर्स तथा फिटीङग्स")]
        FurnitureFixtureFittings = 'F',
        [Description("नगद,सुनचाँदी तथा हिरा जवाहरत")]
        MoneyAndJewellery = 'G',
        [Description("नक्सा ढलाईको सांचो, पन्डुलिपि, चित्रकला, कलात्मक बस्तु तथा दुर्लभ सामग्री")]
        Art = 'H',
        [Description("अन्य सरसामान (प्रत्येक एक लाख रुपैया भन्दा बढी रकम सामानको विवरण खुलाउने )")]
        OtherItems = 'I'
    }
}
