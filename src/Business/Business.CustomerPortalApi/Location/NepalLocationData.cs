using System;
using System.Collections.Generic;
using System.Linq;
using Models.Common.Location;
using Models.Common.Province;

namespace Business.BeemaEdgeApi.Province;

public static class NepalLocationData
{
    private static readonly List<ProvinceViewModel> Provinces = new()
    {
        new ProvinceViewModel { Code = "P1", Name = "Province 1", NameNp = "प्रदेश १" },
        new ProvinceViewModel { Code = "P2", Name = "Madhesh Province", NameNp = "मधेश प्रदेश" },
        new ProvinceViewModel { Code = "P3", Name = "Bagmati Province", NameNp = "बागमती प्रदेश" },
        new ProvinceViewModel { Code = "P4", Name = "Gandaki Province", NameNp = "गण्डकी प्रदेश" },
        new ProvinceViewModel { Code = "P5", Name = "Lumbini Province", NameNp = "लुम्बिनी प्रदेश" },
        new ProvinceViewModel { Code = "P6", Name = "Karnali Province", NameNp = "कर्णाली प्रदेश" },
        new ProvinceViewModel { Code = "P7", Name = "Sudurpashchim Province", NameNp = "सुदूरपश्चिम प्रदेश" }
    };

    private static readonly List<DistrictViewModel> Districts = new()
    {
        // Province 1
        new DistrictViewModel { Code = "D0101", ProvinceCode = "P1", ProvinceName = "Province 1", Name = "Bhojpur", NameNp = "भोजपुर" },
        new DistrictViewModel { Code = "D0102", ProvinceCode = "P1", ProvinceName = "Province 1", Name = "Dhankuta", NameNp = "धनकुटा" },
        new DistrictViewModel { Code = "D0103", ProvinceCode = "P1", ProvinceName = "Province 1", Name = "Ilam", NameNp = "इलाम" },
        new DistrictViewModel { Code = "D0104", ProvinceCode = "P1", ProvinceName = "Province 1", Name = "Jhapa", NameNp = "झापा" },
        new DistrictViewModel { Code = "D0105", ProvinceCode = "P1", ProvinceName = "Province 1", Name = "Khotang", NameNp = "खोटाङ" },
        new DistrictViewModel { Code = "D0106", ProvinceCode = "P1", ProvinceName = "Province 1", Name = "Morang", NameNp = "मोरङ" },
        new DistrictViewModel { Code = "D0107", ProvinceCode = "P1", ProvinceName = "Province 1", Name = "Okhaldhunga", NameNp = "ओखलढुङ्गा" },
        new DistrictViewModel { Code = "D0108", ProvinceCode = "P1", ProvinceName = "Province 1", Name = "Panchthar", NameNp = "पाँचथर" },
        new DistrictViewModel { Code = "D0109", ProvinceCode = "P1", ProvinceName = "Province 1", Name = "Sankhuwasabha", NameNp = "सङ्खुवासभा" },
        new DistrictViewModel { Code = "D0110", ProvinceCode = "P1", ProvinceName = "Province 1", Name = "Solukhumbu", NameNp = "सोलुखुम्बु" },
        new DistrictViewModel { Code = "D0111", ProvinceCode = "P1", ProvinceName = "Province 1", Name = "Sunsari", NameNp = "सुनसरी" },
        new DistrictViewModel { Code = "D0112", ProvinceCode = "P1", ProvinceName = "Province 1", Name = "Taplejung", NameNp = "ताप्लेजुङ" },
        new DistrictViewModel { Code = "D0113", ProvinceCode = "P1", ProvinceName = "Province 1", Name = "Terhathum", NameNp = "तेह्रथुम" },
        new DistrictViewModel { Code = "D0114", ProvinceCode = "P1", ProvinceName = "Province 1", Name = "Udayapur", NameNp = "उदयपुर" },

        // Madhesh Province
        new DistrictViewModel { Code = "D0201", ProvinceCode = "P2", ProvinceName = "Madhesh Province", Name = "Bara", NameNp = "बारा" },
        new DistrictViewModel { Code = "D0202", ProvinceCode = "P2", ProvinceName = "Madhesh Province", Name = "Dhanusha", NameNp = "धनुषा" },
        new DistrictViewModel { Code = "D0203", ProvinceCode = "P2", ProvinceName = "Madhesh Province", Name = "Mahottari", NameNp = "महोत्तरी" },
        new DistrictViewModel { Code = "D0204", ProvinceCode = "P2", ProvinceName = "Madhesh Province", Name = "Parsa", NameNp = "पर्सा" },
        new DistrictViewModel { Code = "D0205", ProvinceCode = "P2", ProvinceName = "Madhesh Province", Name = "Rautahat", NameNp = "रौतहट" },
        new DistrictViewModel { Code = "D0206", ProvinceCode = "P2", ProvinceName = "Madhesh Province", Name = "Saptari", NameNp = "सप्तरी" },
        new DistrictViewModel { Code = "D0207", ProvinceCode = "P2", ProvinceName = "Madhesh Province", Name = "Sarlahi", NameNp = "सर्लाही" },
        new DistrictViewModel { Code = "D0208", ProvinceCode = "P2", ProvinceName = "Madhesh Province", Name = "Siraha", NameNp = "सिराहा" },

        // Bagmati Province
        new DistrictViewModel { Code = "D0301", ProvinceCode = "P3", ProvinceName = "Bagmati Province", Name = "Bhaktapur", NameNp = "भक्तपुर" },
        new DistrictViewModel { Code = "D0302", ProvinceCode = "P3", ProvinceName = "Bagmati Province", Name = "Chitwan", NameNp = "चितवन" },
        new DistrictViewModel { Code = "D0303", ProvinceCode = "P3", ProvinceName = "Bagmati Province", Name = "Dhading", NameNp = "धादिङ" },
        new DistrictViewModel { Code = "D0304", ProvinceCode = "P3", ProvinceName = "Bagmati Province", Name = "Dolakha", NameNp = "दोलखा" },
        new DistrictViewModel { Code = "D0305", ProvinceCode = "P3", ProvinceName = "Bagmati Province", Name = "Kathmandu", NameNp = "काठमाडौं" },
        new DistrictViewModel { Code = "D0306", ProvinceCode = "P3", ProvinceName = "Bagmati Province", Name = "Kavrepalanchok", NameNp = "काभ्रेपलाञ्चोक" },
        new DistrictViewModel { Code = "D0307", ProvinceCode = "P3", ProvinceName = "Bagmati Province", Name = "Lalitpur", NameNp = "ललितपुर" },
        new DistrictViewModel { Code = "D0308", ProvinceCode = "P3", ProvinceName = "Bagmati Province", Name = "Makwanpur", NameNp = "मकवानपुर" },
        new DistrictViewModel { Code = "D0309", ProvinceCode = "P3", ProvinceName = "Bagmati Province", Name = "Nuwakot", NameNp = "नुवाकोट" },
        new DistrictViewModel { Code = "D0310", ProvinceCode = "P3", ProvinceName = "Bagmati Province", Name = "Ramechhap", NameNp = "रामेछाप" },
        new DistrictViewModel { Code = "D0311", ProvinceCode = "P3", ProvinceName = "Bagmati Province", Name = "Rasuwa", NameNp = "रसुवा" },
        new DistrictViewModel { Code = "D0312", ProvinceCode = "P3", ProvinceName = "Bagmati Province", Name = "Sindhuli", NameNp = "सिन्धुली" },
        new DistrictViewModel { Code = "D0313", ProvinceCode = "P3", ProvinceName = "Bagmati Province", Name = "Sindhupalchok", NameNp = "सिन्धुपाल्चोक" },

        // Gandaki Province
        new DistrictViewModel { Code = "D0401", ProvinceCode = "P4", ProvinceName = "Gandaki Province", Name = "Baglung", NameNp = "बागलुङ" },
        new DistrictViewModel { Code = "D0402", ProvinceCode = "P4", ProvinceName = "Gandaki Province", Name = "Gorkha", NameNp = "गोरखा" },
        new DistrictViewModel { Code = "D0403", ProvinceCode = "P4", ProvinceName = "Gandaki Province", Name = "Kaski", NameNp = "कास्की" },
        new DistrictViewModel { Code = "D0404", ProvinceCode = "P4", ProvinceName = "Gandaki Province", Name = "Lamjung", NameNp = "लमजुङ" },
        new DistrictViewModel { Code = "D0405", ProvinceCode = "P4", ProvinceName = "Gandaki Province", Name = "Manang", NameNp = "मनाङ" },
        new DistrictViewModel { Code = "D0406", ProvinceCode = "P4", ProvinceName = "Gandaki Province", Name = "Mustang", NameNp = "मुस्ताङ" },
        new DistrictViewModel { Code = "D0407", ProvinceCode = "P4", ProvinceName = "Gandaki Province", Name = "Myagdi", NameNp = "म्याग्दी" },
        new DistrictViewModel { Code = "D0408", ProvinceCode = "P4", ProvinceName = "Gandaki Province", Name = "Nawalpur", NameNp = "नवलपुर" },
        new DistrictViewModel { Code = "D0409", ProvinceCode = "P4", ProvinceName = "Gandaki Province", Name = "Parbat", NameNp = "पर्वत" },
        new DistrictViewModel { Code = "D0410", ProvinceCode = "P4", ProvinceName = "Gandaki Province", Name = "Syangja", NameNp = "स्याङ्जा" },
        new DistrictViewModel { Code = "D0411", ProvinceCode = "P4", ProvinceName = "Gandaki Province", Name = "Tanahun", NameNp = "तनहुँ" },

        // Lumbini Province
        new DistrictViewModel { Code = "D0501", ProvinceCode = "P5", ProvinceName = "Lumbini Province", Name = "Arghakhanchi", NameNp = "अर्घाखाँची" },
        new DistrictViewModel { Code = "D0502", ProvinceCode = "P5", ProvinceName = "Lumbini Province", Name = "Banke", NameNp = "बाँके" },
        new DistrictViewModel { Code = "D0503", ProvinceCode = "P5", ProvinceName = "Lumbini Province", Name = "Bardiya", NameNp = "बर्दिया" },
        new DistrictViewModel { Code = "D0504", ProvinceCode = "P5", ProvinceName = "Lumbini Province", Name = "Dang", NameNp = "दाङ" },
        new DistrictViewModel { Code = "D0505", ProvinceCode = "P5", ProvinceName = "Lumbini Province", Name = "Gulmi", NameNp = "गुल्मी" },
        new DistrictViewModel { Code = "D0506", ProvinceCode = "P5", ProvinceName = "Lumbini Province", Name = "Kapilvastu", NameNp = "कपिलवस्तु" },
        new DistrictViewModel { Code = "D0507", ProvinceCode = "P5", ProvinceName = "Lumbini Province", Name = "Nawalparasi East", NameNp = "नवलपरासी पूर्व" },
        new DistrictViewModel { Code = "D0508", ProvinceCode = "P5", ProvinceName = "Lumbini Province", Name = "Palpa", NameNp = "पाल्पा" },
        new DistrictViewModel { Code = "D0509", ProvinceCode = "P5", ProvinceName = "Lumbini Province", Name = "Pyuthan", NameNp = "प्युठान" },
        new DistrictViewModel { Code = "D0510", ProvinceCode = "P5", ProvinceName = "Lumbini Province", Name = "Rolpa", NameNp = "रोल्पा" },
        new DistrictViewModel { Code = "D0511", ProvinceCode = "P5", ProvinceName = "Lumbini Province", Name = "Rukum East", NameNp = "रुकुम पूर्व" },
        new DistrictViewModel { Code = "D0512", ProvinceCode = "P5", ProvinceName = "Lumbini Province", Name = "Rupandehi", NameNp = "रुपन्देही" },

        // Karnali Province
        new DistrictViewModel { Code = "D0601", ProvinceCode = "P6", ProvinceName = "Karnali Province", Name = "Dailekh", NameNp = "दैलेख" },
        new DistrictViewModel { Code = "D0602", ProvinceCode = "P6", ProvinceName = "Karnali Province", Name = "Dolpa", NameNp = "डोल्पा" },
        new DistrictViewModel { Code = "D0603", ProvinceCode = "P6", ProvinceName = "Karnali Province", Name = "Humla", NameNp = "हुम्ला" },
        new DistrictViewModel { Code = "D0604", ProvinceCode = "P6", ProvinceName = "Karnali Province", Name = "Jajarkot", NameNp = "जाजरकोट" },
        new DistrictViewModel { Code = "D0605", ProvinceCode = "P6", ProvinceName = "Karnali Province", Name = "Jumla", NameNp = "जुम्ला" },
        new DistrictViewModel { Code = "D0606", ProvinceCode = "P6", ProvinceName = "Karnali Province", Name = "Kalikot", NameNp = "कालिकोट" },
        new DistrictViewModel { Code = "D0607", ProvinceCode = "P6", ProvinceName = "Karnali Province", Name = "Mugu", NameNp = "मुगु" },
        new DistrictViewModel { Code = "D0608", ProvinceCode = "P6", ProvinceName = "Karnali Province", Name = "Rukum West", NameNp = "रुकुम पश्चिम" },
        new DistrictViewModel { Code = "D0609", ProvinceCode = "P6", ProvinceName = "Karnali Province", Name = "Salyan", NameNp = "सल्यान" },
        new DistrictViewModel { Code = "D0610", ProvinceCode = "P6", ProvinceName = "Karnali Province", Name = "Surkhet", NameNp = "सुर्खेत" },

        // Sudurpashchim Province
        new DistrictViewModel { Code = "D0701", ProvinceCode = "P7", ProvinceName = "Sudurpashchim Province", Name = "Achham", NameNp = "अछाम" },
        new DistrictViewModel { Code = "D0702", ProvinceCode = "P7", ProvinceName = "Sudurpashchim Province", Name = "Baitadi", NameNp = "बैतडी" },
        new DistrictViewModel { Code = "D0703", ProvinceCode = "P7", ProvinceName = "Sudurpashchim Province", Name = "Bajhang", NameNp = "बझाङ" },
        new DistrictViewModel { Code = "D0704", ProvinceCode = "P7", ProvinceName = "Sudurpashchim Province", Name = "Bajura", NameNp = "बाजुरा" },
        new DistrictViewModel { Code = "D0705", ProvinceCode = "P7", ProvinceName = "Sudurpashchim Province", Name = "Dadeldhura", NameNp = "डडेलधुरा" },
        new DistrictViewModel { Code = "D0706", ProvinceCode = "P7", ProvinceName = "Sudurpashchim Province", Name = "Darchula", NameNp = "दार्चुला" },
        new DistrictViewModel { Code = "D0707", ProvinceCode = "P7", ProvinceName = "Sudurpashchim Province", Name = "Doti", NameNp = "डोटी" },
        new DistrictViewModel { Code = "D0708", ProvinceCode = "P7", ProvinceName = "Sudurpashchim Province", Name = "Kailali", NameNp = "कैलाली" },
        new DistrictViewModel { Code = "D0709", ProvinceCode = "P7", ProvinceName = "Sudurpashchim Province", Name = "Kanchanpur", NameNp = "कञ्चनपुर" }
    };

    private static readonly List<MunicipalityViewModel> Municipalities = new()
    {
        // Kathmandu District
        new MunicipalityViewModel { Code = "M030501", DistrictCode = "D0305", DistrictName = "Kathmandu", Name = "Kathmandu Metropolitan City", NameNp = "काठमाडौं महानगरपालिका" },
        new MunicipalityViewModel { Code = "M030502", DistrictCode = "D0305", DistrictName = "Kathmandu", Name = "Budhanilkantha Municipality", NameNp = "बुढानिलकण्ठ नगरपालिका" },
        new MunicipalityViewModel { Code = "M030503", DistrictCode = "D0305", DistrictName = "Kathmandu", Name = "Chandragiri Municipality", NameNp = "चन्द्रागिरी नगरपालिका" },
        new MunicipalityViewModel { Code = "M030504", DistrictCode = "D0305", DistrictName = "Kathmandu", Name = "Dakshinkali Municipality", NameNp = "दक्षिणकाली नगरपालिका" },
        new MunicipalityViewModel { Code = "M030505", DistrictCode = "D0305", DistrictName = "Kathmandu", Name = "Gokarneshwor Municipality", NameNp = "गोकर्णेश्वर नगरपालिका" },
        new MunicipalityViewModel { Code = "M030506", DistrictCode = "D0305", DistrictName = "Kathmandu", Name = "Kageshwori Manohara Municipality", NameNp = "कागेश्वरी मनोहरा नगरपालिका" },
        new MunicipalityViewModel { Code = "M030507", DistrictCode = "D0305", DistrictName = "Kathmandu", Name = "Kirtipur Municipality", NameNp = "कीर्तिपुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M030508", DistrictCode = "D0305", DistrictName = "Kathmandu", Name = "Nagarjun Municipality", NameNp = "नागार्जुन नगरपालिका" },
        new MunicipalityViewModel { Code = "M030509", DistrictCode = "D0305", DistrictName = "Kathmandu", Name = "Shankharapur Municipality", NameNp = "शङ्खरापुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M030510", DistrictCode = "D0305", DistrictName = "Kathmandu", Name = "Tarakeshwor Municipality", NameNp = "तारकेश्वर नगरपालिका" },
        new MunicipalityViewModel { Code = "M030511", DistrictCode = "D0305", DistrictName = "Kathmandu", Name = "Tokha Municipality", NameNp = "टोखा नगरपालिका" },

        // Lalitpur District
        new MunicipalityViewModel { Code = "M030701", DistrictCode = "D0307", DistrictName = "Lalitpur", Name = "Lalitpur Metropolitan City", NameNp = "ललितपुर महानगरपालिका" },
        new MunicipalityViewModel { Code = "M030702", DistrictCode = "D0307", DistrictName = "Lalitpur", Name = "Godawari Municipality", NameNp = "गोदावरी नगरपालिका" },
        new MunicipalityViewModel { Code = "M030703", DistrictCode = "D0307", DistrictName = "Lalitpur", Name = "Mahalaxmi Municipality", NameNp = "महालक्ष्मी नगरपालिका" },
        new MunicipalityViewModel { Code = "M030704", DistrictCode = "D0307", DistrictName = "Lalitpur", Name = "Konjyosom Rural Municipality", NameNp = "कोंज्योसोम गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030705", DistrictCode = "D0307", DistrictName = "Lalitpur", Name = "Bagmati Rural Municipality", NameNp = "बागमती गाउँपालिका" },

        // Bhaktapur District
        new MunicipalityViewModel { Code = "M030101", DistrictCode = "D0301", DistrictName = "Bhaktapur", Name = "Bhaktapur Municipality", NameNp = "भक्तपुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M030102", DistrictCode = "D0301", DistrictName = "Bhaktapur", Name = "Changunarayan Municipality", NameNp = "चङ्गुनारायण नगरपालिका" },
        new MunicipalityViewModel { Code = "M030103", DistrictCode = "D0301", DistrictName = "Bhaktapur", Name = "Madhyapur Thimi Municipality", NameNp = "मध्यपुर थिमी नगरपालिका" },
        new MunicipalityViewModel { Code = "M030104", DistrictCode = "D0301", DistrictName = "Bhaktapur", Name = "Suryabinayak Municipality", NameNp = "सूर्यविनायक नगरपालिका" },

        // Chitwan District
        new MunicipalityViewModel { Code = "M030201", DistrictCode = "D0302", DistrictName = "Chitwan", Name = "Bharatpur Metropolitan City", NameNp = "भरतपुर महानगरपालिका" },
        new MunicipalityViewModel { Code = "M030202", DistrictCode = "D0302", DistrictName = "Chitwan", Name = "Kalika Municipality", NameNp = "कालिका नगरपालिका" },
        new MunicipalityViewModel { Code = "M030203", DistrictCode = "D0302", DistrictName = "Chitwan", Name = "Khairhani Municipality", NameNp = "खैरहनी नगरपालिका" },
        new MunicipalityViewModel { Code = "M030204", DistrictCode = "D0302", DistrictName = "Chitwan", Name = "Madi Municipality", NameNp = "माडी नगरपालिका" },
        new MunicipalityViewModel { Code = "M030205", DistrictCode = "D0302", DistrictName = "Chitwan", Name = "Ratnanagar Municipality", NameNp = "रत्ननगर नगरपालिका" },

        // Kaski District
        new MunicipalityViewModel { Code = "M040301", DistrictCode = "D0403", DistrictName = "Kaski", Name = "Pokhara Metropolitan City", NameNp = "पोखरा महानगरपालिका" },
        new MunicipalityViewModel { Code = "M040302", DistrictCode = "D0403", DistrictName = "Kaski", Name = "Annapurna Rural Municipality", NameNp = "अन्नपूर्ण गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040303", DistrictCode = "D0403", DistrictName = "Kaski", Name = "Machhapuchchhre Rural Municipality", NameNp = "माछापुछ्रे गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040304", DistrictCode = "D0403", DistrictName = "Kaski", Name = "Madi Rural Municipality", NameNp = "माडी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040305", DistrictCode = "D0403", DistrictName = "Kaski", Name = "Rupa Rural Municipality", NameNp = "रुपा गाउँपालिका" },

        // Morang District
        new MunicipalityViewModel { Code = "M010601", DistrictCode = "D0106", DistrictName = "Morang", Name = "Biratnagar Metropolitan City", NameNp = "विराटनगर महानगरपालिका" },
        new MunicipalityViewModel { Code = "M010602", DistrictCode = "D0106", DistrictName = "Morang", Name = "Belbari Municipality", NameNp = "बेलवारी नगरपालिका" },
        new MunicipalityViewModel { Code = "M010603", DistrictCode = "D0106", DistrictName = "Morang", Name = "Biratnagar Sub-Metropolitan City", NameNp = "विराटनगर उपमहानगरपालिका" },
        new MunicipalityViewModel { Code = "M010604", DistrictCode = "D0106", DistrictName = "Morang", Name = "Gramthan Rural Municipality", NameNp = "ग्रामथान गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010605", DistrictCode = "D0106", DistrictName = "Morang", Name = "Jahada Rural Municipality", NameNp = "जहदा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010606", DistrictCode = "D0106", DistrictName = "Morang", Name = "Kanepokhari Rural Municipality", NameNp = "कानेपोखरी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010607", DistrictCode = "D0106", DistrictName = "Morang", Name = "Kerabari Rural Municipality", NameNp = "केराबारी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010608", DistrictCode = "D0106", DistrictName = "Morang", Name = "Letang Municipality", NameNp = "लेटाङ नगरपालिका" },
        new MunicipalityViewModel { Code = "M010609", DistrictCode = "D0106", DistrictName = "Morang", Name = "Miklajung Rural Municipality", NameNp = "मिक्लाजुङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010610", DistrictCode = "D0106", DistrictName = "Morang", Name = "Patahrishanishchare Municipality", NameNp = "पथरीशनिश्चरे नगरपालिका" },
        new MunicipalityViewModel { Code = "M010611", DistrictCode = "D0106", DistrictName = "Morang", Name = "Rangeli Municipality", NameNp = "रंगेली नगरपालिका" },
        new MunicipalityViewModel { Code = "M010612", DistrictCode = "D0106", DistrictName = "Morang", Name = "Ratuwamai Municipality", NameNp = "रतुवामाई नगरपालिका" },
        new MunicipalityViewModel { Code = "M010613", DistrictCode = "D0106", DistrictName = "Morang", Name = "Sundarharaicha Municipality", NameNp = "सुन्दरहरैचा नगरपालिका" },
        new MunicipalityViewModel { Code = "M010614", DistrictCode = "D0106", DistrictName = "Morang", Name = "Sunwarshi Municipality", NameNp = "सुनवर्षी नगरपालिका" },
        new MunicipalityViewModel { Code = "M010615", DistrictCode = "D0106", DistrictName = "Morang", Name = "Urlabari Municipality", NameNp = "उर्लाबारी नगरपालिका" },

        // Jhapa District
        new MunicipalityViewModel { Code = "M010401", DistrictCode = "D0104", DistrictName = "Jhapa", Name = "Bhadrapur Municipality", NameNp = "भद्रपुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M010402", DistrictCode = "D0104", DistrictName = "Jhapa", Name = "Birtamod Municipality", NameNp = "विर्तामोड नगरपालिका" },
        new MunicipalityViewModel { Code = "M010403", DistrictCode = "D0104", DistrictName = "Jhapa", Name = "Buddhashanti Rural Municipality", NameNp = "बुद्धशान्ति गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010404", DistrictCode = "D0104", DistrictName = "Jhapa", Name = "Damak Municipality", NameNp = "दमक नगरपालिका" },
        new MunicipalityViewModel { Code = "M010405", DistrictCode = "D0104", DistrictName = "Jhapa", Name = "Gauradaha Municipality", NameNp = "गौरादह नगरपालिका" },
        new MunicipalityViewModel { Code = "M010406", DistrictCode = "D0104", DistrictName = "Jhapa", Name = "Haldibari Rural Municipality", NameNp = "हल्दीबारी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010407", DistrictCode = "D0104", DistrictName = "Jhapa", Name = "Kachankawal Rural Municipality", NameNp = "कचनकवल गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010408", DistrictCode = "D0104", DistrictName = "Jhapa", Name = "Kamal Rural Municipality", NameNp = "कमल गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010409", DistrictCode = "D0104", DistrictName = "Jhapa", Name = "Kankai Municipality", NameNp = "कन्काई नगरपालिका" },
        new MunicipalityViewModel { Code = "M010410", DistrictCode = "D0104", DistrictName = "Jhapa", Name = "Mechinagar Municipality", NameNp = "मेचीनगर नगरपालिका" },
        new MunicipalityViewModel { Code = "M010411", DistrictCode = "D0104", DistrictName = "Jhapa", Name = "Shivasatakshi Municipality", NameNp = "शिवसताक्षी नगरपालिका" },

        // Province 1 - Remaining Districts
        // Bhojpur District
        new MunicipalityViewModel { Code = "M010101", DistrictCode = "D0101", DistrictName = "Bhojpur", Name = "Bhojpur Municipality", NameNp = "भोजपुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M010102", DistrictCode = "D0101", DistrictName = "Bhojpur", Name = "Shadananda Rural Municipality", NameNp = "शदानन्द गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010103", DistrictCode = "D0101", DistrictName = "Bhojpur", Name = "Aamchowk Rural Municipality", NameNp = "आमचोक गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010104", DistrictCode = "D0101", DistrictName = "Bhojpur", Name = "Ramprasad Rai Rural Municipality", NameNp = "रामप्रसाद राई गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010105", DistrictCode = "D0101", DistrictName = "Bhojpur", Name = "Arun Rural Municipality", NameNp = "अरुण गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010106", DistrictCode = "D0101", DistrictName = "Bhojpur", Name = "Pauwadungma Rural Municipality", NameNp = "पौवादुङमा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010107", DistrictCode = "D0101", DistrictName = "Bhojpur", Name = "Temkemaiyung Rural Municipality", NameNp = "टेम्केमैयुङ गाउँपालिका" },

        // Dhankuta District
        new MunicipalityViewModel { Code = "M010201", DistrictCode = "D0102", DistrictName = "Dhankuta", Name = "Dhankuta Municipality", NameNp = "धनकुटा नगरपालिका" },
        new MunicipalityViewModel { Code = "M010202", DistrictCode = "D0102", DistrictName = "Dhankuta", Name = "Pakhribas Municipality", NameNp = "पाख्रिबास नगरपालिका" },
        new MunicipalityViewModel { Code = "M010203", DistrictCode = "D0102", DistrictName = "Dhankuta", Name = "Mahalaxmi Municipality", NameNp = "महालक्ष्मी नगरपालिका" },
        new MunicipalityViewModel { Code = "M010204", DistrictCode = "D0102", DistrictName = "Dhankuta", Name = "Sangurigadhi Rural Municipality", NameNp = "साङुरीगढी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010205", DistrictCode = "D0102", DistrictName = "Dhankuta", Name = "Chhathar Jorpati Rural Municipality", NameNp = "छथर जोरपाटी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010206", DistrictCode = "D0102", DistrictName = "Dhankuta", Name = "Chaubise Rural Municipality", NameNp = "चौबिसे गाउँपालिका" },

        // Ilam District
        new MunicipalityViewModel { Code = "M010301", DistrictCode = "D0103", DistrictName = "Ilam", Name = "Ilam Municipality", NameNp = "इलाम नगरपालिका" },
        new MunicipalityViewModel { Code = "M010302", DistrictCode = "D0103", DistrictName = "Ilam", Name = "Deumai Municipality", NameNp = "देउमाई नगरपालिका" },
        new MunicipalityViewModel { Code = "M010303", DistrictCode = "D0103", DistrictName = "Ilam", Name = "Mai Municipality", NameNp = "माई नगरपालिका" },
        new MunicipalityViewModel { Code = "M010304", DistrictCode = "D0103", DistrictName = "Ilam", Name = "Phakphokthum Rural Municipality", NameNp = "फाकफोकथुम गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010305", DistrictCode = "D0103", DistrictName = "Ilam", Name = "Mai Jogmai Rural Municipality", NameNp = "माई जोगमाई गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010306", DistrictCode = "D0103", DistrictName = "Ilam", Name = "Chulachuli Rural Municipality", NameNp = "चुलाचुली गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010307", DistrictCode = "D0103", DistrictName = "Ilam", Name = "Rong Rural Municipality", NameNp = "रोङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010308", DistrictCode = "D0103", DistrictName = "Ilam", Name = "Sandakpur Rural Municipality", NameNp = "सन्दकपुर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010309", DistrictCode = "D0103", DistrictName = "Ilam", Name = "Mangsebung Rural Municipality", NameNp = "माङसेबुङ गाउँपालिका" },

        // Khotang District
        new MunicipalityViewModel { Code = "M010501", DistrictCode = "D0105", DistrictName = "Khotang", Name = "Diktel Rupakot Majhuwagadhi Municipality", NameNp = "दिक्तेल रुपाकोट मझुवागढी नगरपालिका" },
        new MunicipalityViewModel { Code = "M010502", DistrictCode = "D0105", DistrictName = "Khotang", Name = "Halesi Tuwachung Municipality", NameNp = "हलेसी तुवाचुङ नगरपालिका" },
        new MunicipalityViewModel { Code = "M010503", DistrictCode = "D0105", DistrictName = "Khotang", Name = "Khotehang Rural Municipality", NameNp = "खोटेहाङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010504", DistrictCode = "D0105", DistrictName = "Khotang", Name = "Diprung Chuichumma Rural Municipality", NameNp = "दिप्रुङ चुइचुम्मा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010505", DistrictCode = "D0105", DistrictName = "Khotang", Name = "Aiselukharka Rural Municipality", NameNp = "ऐसेलुखर्क गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010506", DistrictCode = "D0105", DistrictName = "Khotang", Name = "Jantedhunga Rural Municipality", NameNp = "जन्तेधुङा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010507", DistrictCode = "D0105", DistrictName = "Khotang", Name = "Kepilasagadhi Rural Municipality", NameNp = "केपिलासगढी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010508", DistrictCode = "D0105", DistrictName = "Khotang", Name = "Barahapokhari Rural Municipality", NameNp = "बराहपोखरी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010509", DistrictCode = "D0105", DistrictName = "Khotang", Name = "Rawabesi Rural Municipality", NameNp = "रवाबेसी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010510", DistrictCode = "D0105", DistrictName = "Khotang", Name = "Sakela Rural Municipality", NameNp = "साकेला गाउँपालिका" },

        // Okhaldhunga District
        new MunicipalityViewModel { Code = "M010701", DistrictCode = "D0107", DistrictName = "Okhaldhunga", Name = "Siddhicharan Municipality", NameNp = "सिद्धिचरण नगरपालिका" },
        new MunicipalityViewModel { Code = "M010702", DistrictCode = "D0107", DistrictName = "Okhaldhunga", Name = "Manebhanjyang Rural Municipality", NameNp = "मानेभञ्ज्याङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010703", DistrictCode = "D0107", DistrictName = "Okhaldhunga", Name = "Sunkoshi Rural Municipality", NameNp = "सुनकोशी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010704", DistrictCode = "D0107", DistrictName = "Okhaldhunga", Name = "Champadevi Rural Municipality", NameNp = "चम्पादेवी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010705", DistrictCode = "D0107", DistrictName = "Okhaldhunga", Name = "Molung Rural Municipality", NameNp = "मोलुङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010706", DistrictCode = "D0107", DistrictName = "Okhaldhunga", Name = "Likhu Rural Municipality", NameNp = "लिखु गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010707", DistrictCode = "D0107", DistrictName = "Okhaldhunga", Name = "Udayapurgadhi Rural Municipality", NameNp = "उदयपurgढी गाउँपालिका" },

        // Panchthar District
        new MunicipalityViewModel { Code = "M010801", DistrictCode = "D0108", DistrictName = "Panchthar", Name = "Phidim Municipality", NameNp = "फिदिम नगरपालिका" },
        new MunicipalityViewModel { Code = "M010802", DistrictCode = "D0108", DistrictName = "Panchthar", Name = "Hilihang Rural Municipality", NameNp = "हिलिहाङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010803", DistrictCode = "D0108", DistrictName = "Panchthar", Name = "Kummayak Rural Municipality", NameNp = "कुम्मायक गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010804", DistrictCode = "D0108", DistrictName = "Panchthar", Name = "Miklajung Rural Municipality", NameNp = "मिक्लाजुङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010805", DistrictCode = "D0108", DistrictName = "Panchthar", Name = "Phalelung Rural Municipality", NameNp = "फालेलुङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010806", DistrictCode = "D0108", DistrictName = "Panchthar", Name = "Tumbewa Rural Municipality", NameNp = "तुम्बेवा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010807", DistrictCode = "D0108", DistrictName = "Panchthar", Name = "Yangwarak Rural Municipality", NameNp = "याङवरक गाउँपालिका" },

        // Sankhuwasabha District
        new MunicipalityViewModel { Code = "M010901", DistrictCode = "D0109", DistrictName = "Sankhuwasabha", Name = "Khandbari Municipality", NameNp = "खाँदवारी नगरपालिका" },
        new MunicipalityViewModel { Code = "M010902", DistrictCode = "D0109", DistrictName = "Sankhuwasabha", Name = "Chainpur Municipality", NameNp = "चैनपुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M010903", DistrictCode = "D0109", DistrictName = "Sankhuwasabha", Name = "Dharmadevi Municipality", NameNp = "धर्मदेवी नगरपालिका" },
        new MunicipalityViewModel { Code = "M010904", DistrictCode = "D0109", DistrictName = "Sankhuwasabha", Name = "Madi Municipality", NameNp = "माडी नगरपालिका" },
        new MunicipalityViewModel { Code = "M010905", DistrictCode = "D0109", DistrictName = "Sankhuwasabha", Name = "Panchkhapan Municipality", NameNp = "पाँचखपन नगरपालिका" },
        new MunicipalityViewModel { Code = "M010906", DistrictCode = "D0109", DistrictName = "Sankhuwasabha", Name = "Bhotkhola Rural Municipality", NameNp = "भोटखोला गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010907", DistrictCode = "D0109", DistrictName = "Sankhuwasabha", Name = "Chichila Rural Municipality", NameNp = "चिचिला गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010908", DistrictCode = "D0109", DistrictName = "Sankhuwasabha", Name = "Makalu Rural Municipality", NameNp = "मकालु गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010909", DistrictCode = "D0109", DistrictName = "Sankhuwasabha", Name = "Sabhapokhari Rural Municipality", NameNp = "सभापोखरी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M010910", DistrictCode = "D0109", DistrictName = "Sankhuwasabha", Name = "Silichong Rural Municipality", NameNp = "सिलिचोङ गाउँपालिका" },

        // Solukhumbu District
        new MunicipalityViewModel { Code = "M011001", DistrictCode = "D0110", DistrictName = "Solukhumbu", Name = "Salleri Municipality", NameNp = "सल्लेरी नगरपालिका" },
        new MunicipalityViewModel { Code = "M011002", DistrictCode = "D0110", DistrictName = "Solukhumbu", Name = "Dudhkunda Municipality", NameNp = "दुधकुण्ड नगरपालिका" },
        new MunicipalityViewModel { Code = "M011003", DistrictCode = "D0110", DistrictName = "Solukhumbu", Name = "Dudhkoshi Rural Municipality", NameNp = "दुधकोशी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011004", DistrictCode = "D0110", DistrictName = "Solukhumbu", Name = "Khumbupasanglahmu Rural Municipality", NameNp = "खुम्बुपासाङलाह्मू गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011005", DistrictCode = "D0110", DistrictName = "Solukhumbu", Name = "Likhupike Rural Municipality", NameNp = "लिखुपिके गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011006", DistrictCode = "D0110", DistrictName = "Solukhumbu", Name = "Mahakulung Rural Municipality", NameNp = "महाकुलुङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011007", DistrictCode = "D0110", DistrictName = "Solukhumbu", Name = "Nechasalyan Rural Municipality", NameNp = "नेचासल्यान गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011008", DistrictCode = "D0110", DistrictName = "Solukhumbu", Name = "Sotang Rural Municipality", NameNp = "सोताङ गाउँपालिका" },

        // Sunsari District
        new MunicipalityViewModel { Code = "M011101", DistrictCode = "D0111", DistrictName = "Sunsari", Name = "Inaruwa Municipality", NameNp = "इनरुवा नगरपालिका" },
        new MunicipalityViewModel { Code = "M011102", DistrictCode = "D0111", DistrictName = "Sunsari", Name = "Duhabi Municipality", NameNp = "दुहवी नगरपालिका" },
        new MunicipalityViewModel { Code = "M011103", DistrictCode = "D0111", DistrictName = "Sunsari", Name = "Itahari Sub-Metropolitan City", NameNp = "ईटहरी उपमहानगरपालिका" },
        new MunicipalityViewModel { Code = "M011104", DistrictCode = "D0111", DistrictName = "Sunsari", Name = "Dharan Sub-Metropolitan City", NameNp = "धरान उपमहानगरपालिका" },
        new MunicipalityViewModel { Code = "M011105", DistrictCode = "D0111", DistrictName = "Sunsari", Name = "Barah Municipality", NameNp = "बराह नगरपालिका" },
        new MunicipalityViewModel { Code = "M011106", DistrictCode = "D0111", DistrictName = "Sunsari", Name = "Ramdhuni Municipality", NameNp = "रामधुनी नगरपालिका" },
        new MunicipalityViewModel { Code = "M011107", DistrictCode = "D0111", DistrictName = "Sunsari", Name = "Gadhi Rural Municipality", NameNp = "गढी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011108", DistrictCode = "D0111", DistrictName = "Sunsari", Name = "Koshi Rural Municipality", NameNp = "कोशी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011109", DistrictCode = "D0111", DistrictName = "Sunsari", Name = "Barju Rural Municipality", NameNp = "बर्जु गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011110", DistrictCode = "D0111", DistrictName = "Sunsari", Name = "Bhokraha Rural Municipality", NameNp = "भोक्रहा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011111", DistrictCode = "D0111", DistrictName = "Sunsari", Name = "Dewanganj Rural Municipality", NameNp = "देवानगञ्ज गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011112", DistrictCode = "D0111", DistrictName = "Sunsari", Name = "Harinagar Rural Municipality", NameNp = "हरिनगर गाउँपालिका" },

        // Taplejung District
        new MunicipalityViewModel { Code = "M011201", DistrictCode = "D0112", DistrictName = "Taplejung", Name = "Phungling Municipality", NameNp = "फुङ्लिङ नगरपालिका" },
        new MunicipalityViewModel { Code = "M011202", DistrictCode = "D0112", DistrictName = "Taplejung", Name = "Meringden Rural Municipality", NameNp = "मेरिङदेन गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011203", DistrictCode = "D0112", DistrictName = "Taplejung", Name = "Aathrai Tribeni Rural Municipality", NameNp = "आथ्राई त्रिवेणी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011204", DistrictCode = "D0112", DistrictName = "Taplejung", Name = "Pathibhara Yangwarak Rural Municipality", NameNp = "पाथीभरा याङवरक गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011205", DistrictCode = "D0112", DistrictName = "Taplejung", Name = "Maiwakhola Rural Municipality", NameNp = "मैवाखोला गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011206", DistrictCode = "D0112", DistrictName = "Taplejung", Name = "Mikwakhola Rural Municipality", NameNp = "मिक्वाखोला गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011207", DistrictCode = "D0112", DistrictName = "Taplejung", Name = "Sidingwa Rural Municipality", NameNp = "सिडिङ्वा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011208", DistrictCode = "D0112", DistrictName = "Taplejung", Name = "Phaktanglung Rural Municipality", NameNp = "फक्ताङ्लुङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011209", DistrictCode = "D0112", DistrictName = "Taplejung", Name = "Sirijangha Rural Municipality", NameNp = "सिरिजङ्घा गाउँपालिका" },

        // Terhathum District
        new MunicipalityViewModel { Code = "M011301", DistrictCode = "D0113", DistrictName = "Terhathum", Name = "Myanglung Municipality", NameNp = "म्याङ्लुङ नगरपालिका" },
        new MunicipalityViewModel { Code = "M011302", DistrictCode = "D0113", DistrictName = "Terhathum", Name = "Laligurans Municipality", NameNp = "लालिगुराँस नगरपालिका" },
        new MunicipalityViewModel { Code = "M011303", DistrictCode = "D0113", DistrictName = "Terhathum", Name = "Aathrai Rural Municipality", NameNp = "आथ्राई गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011304", DistrictCode = "D0113", DistrictName = "Terhathum", Name = "Chhathar Rural Municipality", NameNp = "छथर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011305", DistrictCode = "D0113", DistrictName = "Terhathum", Name = "Phedap Rural Municipality", NameNp = "फेदाप गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011306", DistrictCode = "D0113", DistrictName = "Terhathum", Name = "Menchayayem Rural Municipality", NameNp = "मेन्चयायेम गाउँपालिका" },

        // Udayapur District
        new MunicipalityViewModel { Code = "M011401", DistrictCode = "D0114", DistrictName = "Udayapur", Name = "Triyuga Municipality", NameNp = "त्रियुगा नगरपालिका" },
        new MunicipalityViewModel { Code = "M011402", DistrictCode = "D0114", DistrictName = "Udayapur", Name = "Katari Municipality", NameNp = "कटारी नगरपालिका" },
        new MunicipalityViewModel { Code = "M011403", DistrictCode = "D0114", DistrictName = "Udayapur", Name = "Chaudandigadhi Municipality", NameNp = "चौदण्डीगढी नगरपालिका" },
        new MunicipalityViewModel { Code = "M011404", DistrictCode = "D0114", DistrictName = "Udayapur", Name = "Belaka Municipality", NameNp = "बेलका नगरपालिका" },
        new MunicipalityViewModel { Code = "M011405", DistrictCode = "D0114", DistrictName = "Udayapur", Name = "Udayapurgadhi Rural Municipality", NameNp = "उदयपurgढी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011406", DistrictCode = "D0114", DistrictName = "Udayapur", Name = "Rautamai Rural Municipality", NameNp = "रौतामाई गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011407", DistrictCode = "D0114", DistrictName = "Udayapur", Name = "Tapli Rural Municipality", NameNp = "ताप्ली गाउँपालिका" },
        new MunicipalityViewModel { Code = "M011408", DistrictCode = "D0114", DistrictName = "Udayapur", Name = "Limchungbung Rural Municipality", NameNp = "लिम्चुङबुङ गाउँपालिका" },

        // Madhesh Province Districts
        // Bara District
        new MunicipalityViewModel { Code = "M020101", DistrictCode = "D0201", DistrictName = "Bara", Name = "Kalaiya Sub-Metropolitan City", NameNp = "कलैया उपमहानगरपालिका" },
        new MunicipalityViewModel { Code = "M020102", DistrictCode = "D0201", DistrictName = "Bara", Name = "Jitpur Simara Sub-Metropolitan City", NameNp = "जीतपुर सिमरा उपमहानगरपालिका" },
        new MunicipalityViewModel { Code = "M020103", DistrictCode = "D0201", DistrictName = "Bara", Name = "Kolhabi Municipality", NameNp = "कोल्हवी नगरपालिका" },
        new MunicipalityViewModel { Code = "M020104", DistrictCode = "D0201", DistrictName = "Bara", Name = "Nijgadh Municipality", NameNp = "निजगढ नगरपालिका" },
        new MunicipalityViewModel { Code = "M020105", DistrictCode = "D0201", DistrictName = "Bara", Name = "Mahagadhimai Municipality", NameNp = "महागढीमाई नगरपालिका" },
        new MunicipalityViewModel { Code = "M020106", DistrictCode = "D0201", DistrictName = "Bara", Name = "Simraungadh Municipality", NameNp = "सिम्रौनगढ नगरपालिका" },
        new MunicipalityViewModel { Code = "M020107", DistrictCode = "D0201", DistrictName = "Bara", Name = "Pacharauta Rural Municipality", NameNp = "पचरौता गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020108", DistrictCode = "D0201", DistrictName = "Bara", Name = "Pheta Rural Municipality", NameNp = "फेटा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020109", DistrictCode = "D0201", DistrictName = "Bara", Name = "Bishrampur Rural Municipality", NameNp = "विश्रामपुर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020110", DistrictCode = "D0201", DistrictName = "Bara", Name = "Suwarna Rural Municipality", NameNp = "सुवर्ण गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020111", DistrictCode = "D0201", DistrictName = "Bara", Name = "Baragadhi Rural Municipality", NameNp = "बरागढी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020112", DistrictCode = "D0201", DistrictName = "Bara", Name = "Karaiyamai Rural Municipality", NameNp = "करैयामाई गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020113", DistrictCode = "D0201", DistrictName = "Bara", Name = "Devtal Rural Municipality", NameNp = "देवताल गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020114", DistrictCode = "D0201", DistrictName = "Bara", Name = "Prasauni Rural Municipality", NameNp = "प्रसौनी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020115", DistrictCode = "D0201", DistrictName = "Bara", Name = "Adarshkotwal Rural Municipality", NameNp = "आदर्शकोटवाल गाउँपालिका" },

        // Dhanusha District
        new MunicipalityViewModel { Code = "M020201", DistrictCode = "D0202", DistrictName = "Dhanusha", Name = "Janakpur Sub-Metropolitan City", NameNp = "जनकपुर उपमहानगरपालिका" },
        new MunicipalityViewModel { Code = "M020202", DistrictCode = "D0202", DistrictName = "Dhanusha", Name = "Chhireshwornath Municipality", NameNp = "छिरेश्वरनाथ नगरपालिका" },
        new MunicipalityViewModel { Code = "M020203", DistrictCode = "D0202", DistrictName = "Dhanusha", Name = "Dhanusadham Municipality", NameNp = "धनुषाधाम नगरपालिका" },
        new MunicipalityViewModel { Code = "M020204", DistrictCode = "D0202", DistrictName = "Dhanusha", Name = "Nagarain Municipality", NameNp = "नगराईन नगरपालिका" },
        new MunicipalityViewModel { Code = "M020205", DistrictCode = "D0202", DistrictName = "Dhanusha", Name = "Bideha Municipality", NameNp = "बिदेह नगरपालिका" },
        new MunicipalityViewModel { Code = "M020206", DistrictCode = "D0202", DistrictName = "Dhanusha", Name = "Mithila Municipality", NameNp = "मिथिला नगरपालिका" },
        new MunicipalityViewModel { Code = "M020207", DistrictCode = "D0202", DistrictName = "Dhanusha", Name = "Sahidnagar Municipality", NameNp = "सहिदनगर नगरपालिका" },
        new MunicipalityViewModel { Code = "M020208", DistrictCode = "D0202", DistrictName = "Dhanusha", Name = "Sabaila Municipality", NameNp = "सबैला नगरपालिका" },
        new MunicipalityViewModel { Code = "M020209", DistrictCode = "D0202", DistrictName = "Dhanusha", Name = "Kamala Municipality", NameNp = "कमला नगरपालिका" },
        new MunicipalityViewModel { Code = "M020210", DistrictCode = "D0202", DistrictName = "Dhanusha", Name = "Mithila Bihari Municipality", NameNp = "मिथिला बिहारी नगरपालिका" },
        new MunicipalityViewModel { Code = "M020211", DistrictCode = "D0202", DistrictName = "Dhanusha", Name = "Lakshminiya Rural Municipality", NameNp = "लक्ष्मीनीया गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020212", DistrictCode = "D0202", DistrictName = "Dhanusha", Name = "Aaurahi Rural Municipality", NameNp = "आउरही गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020213", DistrictCode = "D0202", DistrictName = "Dhanusha", Name = "Bateshwor Rural Municipality", NameNp = "बटेश्वर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020214", DistrictCode = "D0202", DistrictName = "Dhanusha", Name = "Mukhiyapatti Musarmiya Rural Municipality", NameNp = "मुखियापट्टी मुसरमिया गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020215", DistrictCode = "D0202", DistrictName = "Dhanusha", Name = "Dhanauji Rural Municipality", NameNp = "धनौजी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020216", DistrictCode = "D0202", DistrictName = "Dhanusha", Name = "Hansapur Rural Municipality", NameNp = "हंसपुर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020217", DistrictCode = "D0202", DistrictName = "Dhanusha", Name = "Janaknandini Rural Municipality", NameNp = "जनकनन्दिनी गाउँपालिका" },

        // Mahottari District
        new MunicipalityViewModel { Code = "M020301", DistrictCode = "D0203", DistrictName = "Mahottari", Name = "Jaleshwor Municipality", NameNp = "जलेश्वर नगरपालिका" },
        new MunicipalityViewModel { Code = "M020302", DistrictCode = "D0203", DistrictName = "Mahottari", Name = "Bardibas Municipality", NameNp = "बर्दिबास नगरपालिका" },
        new MunicipalityViewModel { Code = "M020303", DistrictCode = "D0203", DistrictName = "Mahottari", Name = "Gaushala Municipality", NameNp = "गौशाला नगरपालिका" },
        new MunicipalityViewModel { Code = "M020304", DistrictCode = "D0203", DistrictName = "Mahottari", Name = "Ekdanra Rural Municipality", NameNp = "एकडान्रा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020305", DistrictCode = "D0203", DistrictName = "Mahottari", Name = "Mahottari Rural Municipality", NameNp = "महोत्तरी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020306", DistrictCode = "D0203", DistrictName = "Mahottari", Name = "Ramgopalpur Municipality", NameNp = "रामगोपालपुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M020307", DistrictCode = "D0203", DistrictName = "Mahottari", Name = "Bhangaha Municipality", NameNp = "भंगाहा नगरपालिका" },
        new MunicipalityViewModel { Code = "M020308", DistrictCode = "D0203", DistrictName = "Mahottari", Name = "Balwa Municipality", NameNp = "बलवा नगरपालिका" },
        new MunicipalityViewModel { Code = "M020309", DistrictCode = "D0203", DistrictName = "Mahottari", Name = "Loharpatti Municipality", NameNp = "लोहरपट्टी नगरपालिका" },
        new MunicipalityViewModel { Code = "M020310", DistrictCode = "D0203", DistrictName = "Mahottari", Name = "Aurahi Municipality", NameNp = "औरही नगरपालिका" },
        new MunicipalityViewModel { Code = "M020311", DistrictCode = "D0203", DistrictName = "Mahottari", Name = "Samsi Rural Municipality", NameNp = "साम्सी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020312", DistrictCode = "D0203", DistrictName = "Mahottari", Name = "Sonama Rural Municipality", NameNp = "सोनमा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020313", DistrictCode = "D0203", DistrictName = "Mahottari", Name = "Pipra Rural Municipality", NameNp = "पिप्रा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020314", DistrictCode = "D0203", DistrictName = "Mahottari", Name = "Matihani Rural Municipality", NameNp = "मटिहानी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020315", DistrictCode = "D0203", DistrictName = "Mahottari", Name = "Dhanauji Rural Municipality", NameNp = "धनौजी गाउँपालिका" },

        // Parsa District
        new MunicipalityViewModel { Code = "M020401", DistrictCode = "D0204", DistrictName = "Parsa", Name = "Birgunj Sub-Metropolitan City", NameNp = "वीरगञ्ज उपमहानगरपालिका" },
        new MunicipalityViewModel { Code = "M020402", DistrictCode = "D0204", DistrictName = "Parsa", Name = "Bahudarmai Municipality", NameNp = "बहुदरमाई नगरपालिका" },
        new MunicipalityViewModel { Code = "M020403", DistrictCode = "D0204", DistrictName = "Parsa", Name = "Pokhariya Municipality", NameNp = "पोखरिया नगरपालिका" },
        new MunicipalityViewModel { Code = "M020404", DistrictCode = "D0204", DistrictName = "Parsa", Name = "Parsagadhi Municipality", NameNp = "पर्सागढी नगरपालिका" },
        new MunicipalityViewModel { Code = "M020405", DistrictCode = "D0204", DistrictName = "Parsa", Name = "Jagarnathpur Rural Municipality", NameNp = "जगरनाथपुर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020406", DistrictCode = "D0204", DistrictName = "Parsa", Name = "Chhipaharmai Rural Municipality", NameNp = "छिपहरमाई गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020407", DistrictCode = "D0204", DistrictName = "Parsa", Name = "Bindabasini Rural Municipality", NameNp = "बिन्दबासिनी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020408", DistrictCode = "D0204", DistrictName = "Parsa", Name = "Paterwa Sugauli Rural Municipality", NameNp = "पटेर्वा सुगौली गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020409", DistrictCode = "D0204", DistrictName = "Parsa", Name = "Jirabhawani Rural Municipality", NameNp = "जिराभवानी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020410", DistrictCode = "D0204", DistrictName = "Parsa", Name = "Kalikamai Rural Municipality", NameNp = "कालिकामाई गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020411", DistrictCode = "D0204", DistrictName = "Parsa", Name = "Pakaha Mainpur Rural Municipality", NameNp = "पकाहा मैनपुर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020412", DistrictCode = "D0204", DistrictName = "Parsa", Name = "Sakhuwa Prasauni Rural Municipality", NameNp = "सखुवा प्रसौनी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020413", DistrictCode = "D0204", DistrictName = "Parsa", Name = "Thori Rural Municipality", NameNp = "थोरी गाउँपालिका" },

        // Rautahat District
        new MunicipalityViewModel { Code = "M020501", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Gaur Municipality", NameNp = "गौर नगरपालिका" },
        new MunicipalityViewModel { Code = "M020502", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Chandrapur Municipality", NameNp = "चन्द्रपुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M020503", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Garuda Municipality", NameNp = "गरुडा नगरपालिका" },
        new MunicipalityViewModel { Code = "M020504", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Baudhimai Municipality", NameNp = "बौधीमाई नगरपालिका" },
        new MunicipalityViewModel { Code = "M020505", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Rajpur Municipality", NameNp = "राजपुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M020506", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Gujara Municipality", NameNp = "गुजरा नगरपालिका" },
        new MunicipalityViewModel { Code = "M020507", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Ishanath Municipality", NameNp = "ईशानाथ नगरपालिका" },
        new MunicipalityViewModel { Code = "M020508", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Katahariya Municipality", NameNp = "कटहरिया नगरपालिका" },
        new MunicipalityViewModel { Code = "M020509", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Madhav Narayan Municipality", NameNp = "माधव नारायण नगरपालिका" },
        new MunicipalityViewModel { Code = "M020510", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Phatuwa Bijayapur Municipality", NameNp = "फतुवा बिजयपुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M020511", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Brindaban Municipality", NameNp = "बृन्दाबन नगरपालिका" },
        new MunicipalityViewModel { Code = "M020512", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Yamunamai Rural Municipality", NameNp = "यमुनामाई गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020513", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Dewahi Gonahi Rural Municipality", NameNp = "देवाही गोनाही गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020514", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Rajdevi Rural Municipality", NameNp = "राजदेवी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020515", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Maulapur Municipality", NameNp = "मौलापुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M020516", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Gadhimai Municipality", NameNp = "गढीमाई नगरपालिका" },
        new MunicipalityViewModel { Code = "M020517", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Paroha Municipality", NameNp = "परोहा नगरपालिका" },
        new MunicipalityViewModel { Code = "M020518", DistrictCode = "D0205", DistrictName = "Rautahat", Name = "Durga Bhagwati Rural Municipality", NameNp = "दुर्गा भगवती गाउँपालिका" },

        // Saptari District
        new MunicipalityViewModel { Code = "M020601", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Rajbiraj Municipality", NameNp = "राजविराज नगरपालिका" },
        new MunicipalityViewModel { Code = "M020602", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Hanumannagar Kankalini Municipality", NameNp = "हनुमाननगर कंकालिनी नगरपालिका" },
        new MunicipalityViewModel { Code = "M020603", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Kanchanrup Municipality", NameNp = "कञ्चनरुप नगरपालिका" },
        new MunicipalityViewModel { Code = "M020604", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Dakneshwori Municipality", NameNp = "दक्नेश्वरी नगरपालिका" },
        new MunicipalityViewModel { Code = "M020605", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Surunga Municipality", NameNp = "सुरुङ्गा नगरपालिका" },
        new MunicipalityViewModel { Code = "M020606", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Shambhunath Municipality", NameNp = "शम्भुनाथ नगरपालिका" },
        new MunicipalityViewModel { Code = "M020607", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Bodebarsain Municipality", NameNp = "बोदेबरसाईन नगरपालिका" },
        new MunicipalityViewModel { Code = "M020608", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Khadak Municipality", NameNp = "खडक नगरपालिका" },
        new MunicipalityViewModel { Code = "M020609", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Shivasatakshi Municipality", NameNp = "शिवसताक्षी नगरपालिका" },
        new MunicipalityViewModel { Code = "M020610", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Tilathi Koiladi Rural Municipality", NameNp = "तिलाठी कोइलाडी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020611", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Rupani Rural Municipality", NameNp = "रुपनी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020612", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Agnisair Krishna Savaran Rural Municipality", NameNp = "अग्निसाइर कृष्ण सवरन गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020613", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Balan-Bihul Rural Municipality", NameNp = "बलान-बिहुल गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020614", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Chhinnamasta Rural Municipality", NameNp = "छिन्नमस्ता गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020615", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Mahadeva Rural Municipality", NameNp = "महादेव गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020616", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Rajanagar Rural Municipality", NameNp = "राजानगर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020617", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Saptakoshi Rural Municipality", NameNp = "सप्तकोशी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020618", DistrictCode = "D0206", DistrictName = "Saptari", Name = "Tirhut Rural Municipality", NameNp = "तिरहुत गाउँपालिका" },

        // Sarlahi District
        new MunicipalityViewModel { Code = "M020701", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Malangwa Municipality", NameNp = "मलङ्गवा नगरपालिका" },
        new MunicipalityViewModel { Code = "M020702", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Lalbandi Municipality", NameNp = "लालबन्दी नगरपालिका" },
        new MunicipalityViewModel { Code = "M020703", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Chandranagar Municipality", NameNp = "चन्द्रनगर नगरपालिका" },
        new MunicipalityViewModel { Code = "M020704", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Barahathawa Municipality", NameNp = "बराहथवा नगरपालिका" },
        new MunicipalityViewModel { Code = "M020705", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Haripur Municipality", NameNp = "हरिपुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M020706", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Ishworpur Municipality", NameNp = "ईश्वरपुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M020707", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Godaita Municipality", NameNp = "गोडैता नगरपालिका" },
        new MunicipalityViewModel { Code = "M020708", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Kabilasi Municipality", NameNp = "कबिलासी नगरपालिका" },
        new MunicipalityViewModel { Code = "M020709", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Bagmati Municipality", NameNp = "बागमती नगरपालिका" },
        new MunicipalityViewModel { Code = "M020710", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Balara Municipality", NameNp = "बलरा नगरपालिका" },
        new MunicipalityViewModel { Code = "M020711", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Ramnagar Rural Municipality", NameNp = "रामनगर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020712", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Chandranagar Rural Municipality", NameNp = "चन्द्रनगर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020713", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Dhankaul Rural Municipality", NameNp = "धनकौल गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020714", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Kaudena Rural Municipality", NameNp = "कौडेना गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020715", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Brahmapuri Rural Municipality", NameNp = "ब्रह्मपुरी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020716", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Parsa Rural Municipality", NameNp = "पर्सा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020717", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Basbariya Rural Municipality", NameNp = "बसबरिया गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020718", DistrictCode = "D0207", DistrictName = "Sarlahi", Name = "Hariwan Municipality", NameNp = "हरिवन नगरपालिका" },

        // Siraha District
        new MunicipalityViewModel { Code = "M020801", DistrictCode = "D0208", DistrictName = "Siraha", Name = "Lahan Municipality", NameNp = "लहान नगरपालिका" },
        new MunicipalityViewModel { Code = "M020802", DistrictCode = "D0208", DistrictName = "Siraha", Name = "Siraha Municipality", NameNp = "सिराहा नगरपालिका" },
        new MunicipalityViewModel { Code = "M020803", DistrictCode = "D0208", DistrictName = "Siraha", Name = "Dhangadhimai Municipality", NameNp = "ढंगाधिमाई नगरपालिका" },
        new MunicipalityViewModel { Code = "M020804", DistrictCode = "D0208", DistrictName = "Siraha", Name = "Mirchaiya Municipality", NameNp = "मिर्चैया नगरपालिका" },
        new MunicipalityViewModel { Code = "M020805", DistrictCode = "D0208", DistrictName = "Siraha", Name = "Golbazar Municipality", NameNp = "गोलबजार नगरपालिका" },
        new MunicipalityViewModel { Code = "M020806", DistrictCode = "D0208", DistrictName = "Siraha", Name = "Karjanha Municipality", NameNp = "कर्जन्हा नगरपालिका" },
        new MunicipalityViewModel { Code = "M020807", DistrictCode = "D0208", DistrictName = "Siraha", Name = "Kalyanpur Municipality", NameNp = "कल्याणपुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M020808", DistrictCode = "D0208", DistrictName = "Siraha", Name = "Sukhipur Municipality", NameNp = "सुखीपुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M020809", DistrictCode = "D0208", DistrictName = "Siraha", Name = "Bhagwanpur Rural Municipality", NameNp = "भगवानपुर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020810", DistrictCode = "D0208", DistrictName = "Siraha", Name = "Aurahi Rural Municipality", NameNp = "औरही गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020811", DistrictCode = "D0208", DistrictName = "Siraha", Name = "Bishnupur Rural Municipality", NameNp = "विष्णुपुर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020812", DistrictCode = "D0208", DistrictName = "Siraha", Name = "Bariyarpatti Rural Municipality", NameNp = "बरियारपट्टी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020813", DistrictCode = "D0208", DistrictName = "Siraha", Name = "Lakshmipur Patari Rural Municipality", NameNp = "लक्ष्मीपुर पटारी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020814", DistrictCode = "D0208", DistrictName = "Siraha", Name = "Naraha Rural Municipality", NameNp = "नरहा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020815", DistrictCode = "D0208", DistrictName = "Siraha", Name = "Navarajpur Rural Municipality", NameNp = "नवराजपुर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M020816", DistrictCode = "D0208", DistrictName = "Siraha", Name = "Sakhuwanankarkatti Rural Municipality", NameNp = "सखुवाननकर्कट्टी गाउँपालिका" },

        // Bagmati Province - Remaining Districts
        // Dhading District
        new MunicipalityViewModel { Code = "M030301", DistrictCode = "D0303", DistrictName = "Dhading", Name = "Nilkantha Municipality", NameNp = "नीलकण्ठ नगरपालिका" },
        new MunicipalityViewModel { Code = "M030302", DistrictCode = "D0303", DistrictName = "Dhading", Name = "Khaniyabas Rural Municipality", NameNp = "खनियाबास गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030303", DistrictCode = "D0303", DistrictName = "Dhading", Name = "Thakre Rural Municipality", NameNp = "थाक्रे गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030304", DistrictCode = "D0303", DistrictName = "Dhading", Name = "Netrakot Rural Municipality", NameNp = "नेत्राकोट गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030305", DistrictCode = "D0303", DistrictName = "Dhading", Name = "Jwalamukhi Rural Municipality", NameNp = "ज्वालामुखी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030306", DistrictCode = "D0303", DistrictName = "Dhading", Name = "Gajuri Rural Municipality", NameNp = "गजुरी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030307", DistrictCode = "D0303", DistrictName = "Dhading", Name = "Galchi Rural Municipality", NameNp = "गल्छी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030308", DistrictCode = "D0303", DistrictName = "Dhading", Name = "Gangajamuna Rural Municipality", NameNp = "गंगाजमुना गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030309", DistrictCode = "D0303", DistrictName = "Dhading", Name = "Rubi Valley Rural Municipality", NameNp = "रूबी भ्याली गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030310", DistrictCode = "D0303", DistrictName = "Dhading", Name = "Siddhalek Rural Municipality", NameNp = "सिद्धलेक गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030311", DistrictCode = "D0303", DistrictName = "Dhading", Name = "Tripurasundari Rural Municipality", NameNp = "त्रिपुरासुन्दरी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030312", DistrictCode = "D0303", DistrictName = "Dhading", Name = "Benighat Rorang Rural Municipality", NameNp = "बेनीघाट रोराङ गाउँपालिका" },

        // Dolakha District
        new MunicipalityViewModel { Code = "M030401", DistrictCode = "D0304", DistrictName = "Dolakha", Name = "Bhimeshwor Municipality", NameNp = "भीमेश्वर नगरपालिका" },
        new MunicipalityViewModel { Code = "M030402", DistrictCode = "D0304", DistrictName = "Dolakha", Name = "Jiri Municipality", NameNp = "जिरी नगरपालिका" },
        new MunicipalityViewModel { Code = "M030403", DistrictCode = "D0304", DistrictName = "Dolakha", Name = "Kalinchok Rural Municipality", NameNp = "कालिन्चोक गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030404", DistrictCode = "D0304", DistrictName = "Dolakha", Name = "Melung Rural Municipality", NameNp = "मेलुङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030405", DistrictCode = "D0304", DistrictName = "Dolakha", Name = "Bigu Rural Municipality", NameNp = "बिगु गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030406", DistrictCode = "D0304", DistrictName = "Dolakha", Name = "Gaurishankar Rural Municipality", NameNp = "गौरीशंकर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030407", DistrictCode = "D0304", DistrictName = "Dolakha", Name = "Baiteshwor Rural Municipality", NameNp = "बैतेश्वर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030408", DistrictCode = "D0304", DistrictName = "Dolakha", Name = "Sailung Rural Municipality", NameNp = "सैलुङ गाउँपालिका" },

        // Kavrepalanchok District
        new MunicipalityViewModel { Code = "M030601", DistrictCode = "D0306", DistrictName = "Kavrepalanchok", Name = "Panauti Municipality", NameNp = "पनौती नगरपालिका" },
        new MunicipalityViewModel { Code = "M030602", DistrictCode = "D0306", DistrictName = "Kavrepalanchok", Name = "Banepa Municipality", NameNp = "बनेपा नगरपालिका" },
        new MunicipalityViewModel { Code = "M030603", DistrictCode = "D0306", DistrictName = "Kavrepalanchok", Name = "Dhulikhel Municipality", NameNp = "धुलिखेल नगरपालिका" },
        new MunicipalityViewModel { Code = "M030604", DistrictCode = "D0306", DistrictName = "Kavrepalanchok", Name = "Namobuddha Municipality", NameNp = "नमोबुद्ध नगरपालिका" },
        new MunicipalityViewModel { Code = "M030605", DistrictCode = "D0306", DistrictName = "Kavrepalanchok", Name = "Panchkhal Municipality", NameNp = "पाँचखाल नगरपालिका" },
        new MunicipalityViewModel { Code = "M030606", DistrictCode = "D0306", DistrictName = "Kavrepalanchok", Name = "Mandandeupur Municipality", NameNp = "मण्डनदेउपुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M030607", DistrictCode = "D0306", DistrictName = "Kavrepalanchok", Name = "Bethanchok Rural Municipality", NameNp = "बेथान्चोक गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030608", DistrictCode = "D0306", DistrictName = "Kavrepalanchok", Name = "Bhumlu Rural Municipality", NameNp = "भुम्लु गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030609", DistrictCode = "D0306", DistrictName = "Kavrepalanchok", Name = "Chaurideurali Rural Municipality", NameNp = "चौरीदेउराली गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030610", DistrictCode = "D0306", DistrictName = "Kavrepalanchok", Name = "Khanikhola Rural Municipality", NameNp = "खानीखोला गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030611", DistrictCode = "D0306", DistrictName = "Kavrepalanchok", Name = "Mahabharat Rural Municipality", NameNp = "महाभारत गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030612", DistrictCode = "D0306", DistrictName = "Kavrepalanchok", Name = "Roshi Rural Municipality", NameNp = "रोशी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030613", DistrictCode = "D0306", DistrictName = "Kavrepalanchok", Name = "Temal Rural Municipality", NameNp = "तेमाल गाउँपालिका" },

        // Makwanpur District
        new MunicipalityViewModel { Code = "M030801", DistrictCode = "D0308", DistrictName = "Makwanpur", Name = "Hetauda Sub-Metropolitan City", NameNp = "हेटौडा उपमहानगरपालिका" },
        new MunicipalityViewModel { Code = "M030802", DistrictCode = "D0308", DistrictName = "Makwanpur", Name = "Thaha Municipality", NameNp = "थाहा नगरपालिका" },
        new MunicipalityViewModel { Code = "M030803", DistrictCode = "D0308", DistrictName = "Makwanpur", Name = "Bhimphedi Rural Municipality", NameNp = "भिम्फेदी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030804", DistrictCode = "D0308", DistrictName = "Makwanpur", Name = "Bakaiya Rural Municipality", NameNp = "बकैया गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030805", DistrictCode = "D0308", DistrictName = "Makwanpur", Name = "Bagmati Rural Municipality", NameNp = "बागमती गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030806", DistrictCode = "D0308", DistrictName = "Makwanpur", Name = "Indrasarowar Rural Municipality", NameNp = "इन्द्रसरोवर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030807", DistrictCode = "D0308", DistrictName = "Makwanpur", Name = "Kailash Rural Municipality", NameNp = "कैलाश गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030808", DistrictCode = "D0308", DistrictName = "Makwanpur", Name = "Manahari Rural Municipality", NameNp = "मनहरी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030809", DistrictCode = "D0308", DistrictName = "Makwanpur", Name = "Raksirang Rural Municipality", NameNp = "रक्सिराङ गाउँपालिका" },

        // Nuwakot District
        new MunicipalityViewModel { Code = "M030901", DistrictCode = "D0309", DistrictName = "Nuwakot", Name = "Bidur Municipality", NameNp = "बिदुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M030902", DistrictCode = "D0309", DistrictName = "Nuwakot", Name = "Belkotgadhi Municipality", NameNp = "बेलकोटगढी नगरपालिका" },
        new MunicipalityViewModel { Code = "M030903", DistrictCode = "D0309", DistrictName = "Nuwakot", Name = "Kakani Rural Municipality", NameNp = "ककनी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030904", DistrictCode = "D0309", DistrictName = "Nuwakot", Name = "Kispang Rural Municipality", NameNp = "किस्पाङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030905", DistrictCode = "D0309", DistrictName = "Nuwakot", Name = "Likhu Rural Municipality", NameNp = "लिखु गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030906", DistrictCode = "D0309", DistrictName = "Nuwakot", Name = "Meghang Rural Municipality", NameNp = "मेघाङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030907", DistrictCode = "D0309", DistrictName = "Nuwakot", Name = "Panchakanya Rural Municipality", NameNp = "पञ्चकन्या गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030908", DistrictCode = "D0309", DistrictName = "Nuwakot", Name = "Shivapuri Rural Municipality", NameNp = "शिवपुरी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030909", DistrictCode = "D0309", DistrictName = "Nuwakot", Name = "Suryagadhi Rural Municipality", NameNp = "सूर्यगढी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030910", DistrictCode = "D0309", DistrictName = "Nuwakot", Name = "Tadi Rural Municipality", NameNp = "तादी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M030911", DistrictCode = "D0309", DistrictName = "Nuwakot", Name = "Tarkeshwor Rural Municipality", NameNp = "तारकेश्वर गाउँपालिका" },

        // Ramechhap District
        new MunicipalityViewModel { Code = "M031001", DistrictCode = "D0310", DistrictName = "Ramechhap", Name = "Manthali Municipality", NameNp = "मन्थली नगरपालिका" },
        new MunicipalityViewModel { Code = "M031002", DistrictCode = "D0310", DistrictName = "Ramechhap", Name = "Ramechhap Municipality", NameNp = "रामेछाप नगरपालिका" },
        new MunicipalityViewModel { Code = "M031003", DistrictCode = "D0310", DistrictName = "Ramechhap", Name = "Umakunda Rural Municipality", NameNp = "उमाकुण्ड गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031004", DistrictCode = "D0310", DistrictName = "Ramechhap", Name = "Khadadevi Rural Municipality", NameNp = "खादादेवी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031005", DistrictCode = "D0310", DistrictName = "Ramechhap", Name = "Doramba Rural Municipality", NameNp = "दोरम्बा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031006", DistrictCode = "D0310", DistrictName = "Ramechhap", Name = "Gokulganga Rural Municipality", NameNp = "गोकुलगंगा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031007", DistrictCode = "D0310", DistrictName = "Ramechhap", Name = "Likhu Rural Municipality", NameNp = "लिखु गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031008", DistrictCode = "D0310", DistrictName = "Ramechhap", Name = "Sunapati Rural Municipality", NameNp = "सुनापती गाउँपालिका" },

        // Rasuwa District
        new MunicipalityViewModel { Code = "M031101", DistrictCode = "D0311", DistrictName = "Rasuwa", Name = "Dhunche Municipality", NameNp = "ढुँचे नगरपालिका" },
        new MunicipalityViewModel { Code = "M031102", DistrictCode = "D0311", DistrictName = "Rasuwa", Name = "Kalika Rural Municipality", NameNp = "कालिका गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031103", DistrictCode = "D0311", DistrictName = "Rasuwa", Name = "Naukunda Rural Municipality", NameNp = "नौकुण्ड गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031104", DistrictCode = "D0311", DistrictName = "Rasuwa", Name = "Parbatikunda Rural Municipality", NameNp = "पार्वतीकुण्ड गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031105", DistrictCode = "D0311", DistrictName = "Rasuwa", Name = "Gosaikunda Rural Municipality", NameNp = "गोसाईकुण्ड गाउँपालिका" },

        // Sindhuli District
        new MunicipalityViewModel { Code = "M031201", DistrictCode = "D0312", DistrictName = "Sindhuli", Name = "Kamalamai Municipality", NameNp = "कमलामाई नगरपालिका" },
        new MunicipalityViewModel { Code = "M031202", DistrictCode = "D0312", DistrictName = "Sindhuli", Name = "Dudhauli Municipality", NameNp = "दुधौली नगरपालिका" },
        new MunicipalityViewModel { Code = "M031203", DistrictCode = "D0312", DistrictName = "Sindhuli", Name = "Golanjor Rural Municipality", NameNp = "गोलाञ्जोर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031204", DistrictCode = "D0312", DistrictName = "Sindhuli", Name = "Ghyanglekh Rural Municipality", NameNp = "घ्याङलेख गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031205", DistrictCode = "D0312", DistrictName = "Sindhuli", Name = "Hariharpurgadhi Rural Municipality", NameNp = "हरिहरपुरगढी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031206", DistrictCode = "D0312", DistrictName = "Sindhuli", Name = "Kalimati Rural Municipality", NameNp = "कालिमाटी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031207", DistrictCode = "D0312", DistrictName = "Sindhuli", Name = "Marin Rural Municipality", NameNp = "मरिङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031208", DistrictCode = "D0312", DistrictName = "Sindhuli", Name = "Phikkal Rural Municipality", NameNp = "फिक्कल गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031209", DistrictCode = "D0312", DistrictName = "Sindhuli", Name = "Sunkoshi Rural Municipality", NameNp = "सुनकोशी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031210", DistrictCode = "D0312", DistrictName = "Sindhuli", Name = "Tinpatan Rural Municipality", NameNp = "तिनपाटन गाउँपालिका" },

        // Sindhupalchok District
        new MunicipalityViewModel { Code = "M031301", DistrictCode = "D0313", DistrictName = "Sindhupalchok", Name = "Chautara Sangachokgadhi Municipality", NameNp = "चौतारा साङगाचोकगढी नगरपालिका" },
        new MunicipalityViewModel { Code = "M031302", DistrictCode = "D0313", DistrictName = "Sindhupalchok", Name = "Barhabise Municipality", NameNp = "बाह्रविसे नगरपालिका" },
        new MunicipalityViewModel { Code = "M031303", DistrictCode = "D0313", DistrictName = "Sindhupalchok", Name = "Melamchi Municipality", NameNp = "मेलम्ची नगरपालिका" },
        new MunicipalityViewModel { Code = "M031304", DistrictCode = "D0313", DistrictName = "Sindhupalchok", Name = "Bhotekoshi Rural Municipality", NameNp = "भोटेकोशी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031305", DistrictCode = "D0313", DistrictName = "Sindhupalchok", Name = "Chautara Sangachokgadhi Rural Municipality", NameNp = "चौतारा साङगाचोकगढी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031306", DistrictCode = "D0313", DistrictName = "Sindhupalchok", Name = "Helambu Rural Municipality", NameNp = "हेलम्बु गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031307", DistrictCode = "D0313", DistrictName = "Sindhupalchok", Name = "Indrawati Rural Municipality", NameNp = "इन्द्रावती गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031308", DistrictCode = "D0313", DistrictName = "Sindhupalchok", Name = "Jugal Rural Municipality", NameNp = "जुगल गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031309", DistrictCode = "D0313", DistrictName = "Sindhupalchok", Name = "Lisankhu Pakhar Rural Municipality", NameNp = "लिसाङ्खु पाखर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031310", DistrictCode = "D0313", DistrictName = "Sindhupalchok", Name = "Panchpokhari Thangpal Rural Municipality", NameNp = "पञ्चपोखरी थाङपाल गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031311", DistrictCode = "D0313", DistrictName = "Sindhupalchok", Name = "Sunkoshi Rural Municipality", NameNp = "सुनकोशी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031312", DistrictCode = "D0313", DistrictName = "Sindhupalchok", Name = "Balephi Rural Municipality", NameNp = "बलेफी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M031313", DistrictCode = "D0313", DistrictName = "Sindhupalchok", Name = "Tripurasundari Rural Municipality", NameNp = "त्रिपुरासुन्दरी गाउँपालिका" },

        // Gandaki Province - Remaining Districts (Kaski already added)
        // Baglung District
        new MunicipalityViewModel { Code = "M040101", DistrictCode = "D0401", DistrictName = "Baglung", Name = "Baglung Municipality", NameNp = "बागलुङ नगरपालिका" },
        new MunicipalityViewModel { Code = "M040102", DistrictCode = "D0401", DistrictName = "Baglung", Name = "Galkot Municipality", NameNp = "गल्कोट नगरपालिका" },
        new MunicipalityViewModel { Code = "M040103", DistrictCode = "D0401", DistrictName = "Baglung", Name = "Jaimini Municipality", NameNp = "जैमिनी नगरपालिका" },
        new MunicipalityViewModel { Code = "M040104", DistrictCode = "D0401", DistrictName = "Baglung", Name = "Dhorpatan Municipality", NameNp = "ढोरपाटन नगरपालिका" },
        new MunicipalityViewModel { Code = "M040105", DistrictCode = "D0401", DistrictName = "Baglung", Name = "Bareng Rural Municipality", NameNp = "बरेङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040106", DistrictCode = "D0401", DistrictName = "Baglung", Name = "Kanthekhola Rural Municipality", NameNp = "कान्थेखोला गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040107", DistrictCode = "D0401", DistrictName = "Baglung", Name = "Taman Khola Rural Municipality", NameNp = "तमान खोला गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040108", DistrictCode = "D0401", DistrictName = "Baglung", Name = "Tara Khola Rural Municipality", NameNp = "तारा खोला गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040109", DistrictCode = "D0401", DistrictName = "Baglung", Name = "Nisikhola Rural Municipality", NameNp = "निसीखोला गाउँपालिका" },

        // Gorkha District
        new MunicipalityViewModel { Code = "M040201", DistrictCode = "D0402", DistrictName = "Gorkha", Name = "Gorkha Municipality", NameNp = "गोरखा नगरपालिका" },
        new MunicipalityViewModel { Code = "M040202", DistrictCode = "D0402", DistrictName = "Gorkha", Name = "Palungtar Municipality", NameNp = "पालुङटार नगरपालिका" },
        new MunicipalityViewModel { Code = "M040203", DistrictCode = "D0402", DistrictName = "Gorkha", Name = "Sulikot Rural Municipality", NameNp = "सुलीकोट गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040204", DistrictCode = "D0402", DistrictName = "Gorkha", Name = "Siranchok Rural Municipality", NameNp = "सिरान्चोक गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040205", DistrictCode = "D0402", DistrictName = "Gorkha", Name = "Aarughat Rural Municipality", NameNp = "आरुघाट गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040206", DistrictCode = "D0402", DistrictName = "Gorkha", Name = "Gandaki Rural Municipality", NameNp = "गण्डकी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040207", DistrictCode = "D0402", DistrictName = "Gorkha", Name = "Chum Nubri Rural Municipality", NameNp = "चुम नुब्री गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040208", DistrictCode = "D0402", DistrictName = "Gorkha", Name = "Dharche Rural Municipality", NameNp = "धार्चे गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040209", DistrictCode = "D0402", DistrictName = "Gorkha", Name = "Bhimsen Rural Municipality", NameNp = "भीमसेन गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040210", DistrictCode = "D0402", DistrictName = "Gorkha", Name = "Sahid Lakhan Rural Municipality", NameNp = "सहिद लखन गाउँपालिका" },

        // Lamjung District
        new MunicipalityViewModel { Code = "M040401", DistrictCode = "D0404", DistrictName = "Lamjung", Name = "Besisahar Municipality", NameNp = "बेसीसहर नगरपालिका" },
        new MunicipalityViewModel { Code = "M040402", DistrictCode = "D0404", DistrictName = "Lamjung", Name = "Madhyanepal Municipality", NameNp = "मध्यनेपाल नगरपालिका" },
        new MunicipalityViewModel { Code = "M040403", DistrictCode = "D0404", DistrictName = "Lamjung", Name = "Rainas Municipality", NameNp = "राइनास नगरपालिका" },
        new MunicipalityViewModel { Code = "M040404", DistrictCode = "D0404", DistrictName = "Lamjung", Name = "Sundarbazar Municipality", NameNp = "सुन्दरबजार नगरपालिका" },
        new MunicipalityViewModel { Code = "M040405", DistrictCode = "D0404", DistrictName = "Lamjung", Name = "Dordi Rural Municipality", NameNp = "दोर्दी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040406", DistrictCode = "D0404", DistrictName = "Lamjung", Name = "Dudhpokhari Rural Municipality", NameNp = "दुधपोखरी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040407", DistrictCode = "D0404", DistrictName = "Lamjung", Name = "Kwholasothar Rural Municipality", NameNp = "क्व्होलासोथर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040408", DistrictCode = "D0404", DistrictName = "Lamjung", Name = "Marsyangdi Rural Municipality", NameNp = "मर्स्याङ्दी गाउँपालिका" },

        // Manang District
        new MunicipalityViewModel { Code = "M040501", DistrictCode = "D0405", DistrictName = "Manang", Name = "Chame Rural Municipality", NameNp = "चामे गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040502", DistrictCode = "D0405", DistrictName = "Manang", Name = "Narphu Rural Municipality", NameNp = "नार्फु गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040503", DistrictCode = "D0405", DistrictName = "Manang", Name = "Nashong Rural Municipality", NameNp = "नासोङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040504", DistrictCode = "D0405", DistrictName = "Manang", Name = "Neshyang Rural Municipality", NameNp = "नेस्याङ गाउँपालिका" },

        // Mustang District
        new MunicipalityViewModel { Code = "M040601", DistrictCode = "D0406", DistrictName = "Mustang", Name = "Gharpajhong Rural Municipality", NameNp = "घर्पजोङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040602", DistrictCode = "D0406", DistrictName = "Mustang", Name = "Thasang Rural Municipality", NameNp = "थासाङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040603", DistrictCode = "D0406", DistrictName = "Mustang", Name = "Lomanthang Rural Municipality", NameNp = "लोमान्थाङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040604", DistrictCode = "D0406", DistrictName = "Mustang", Name = "Dalome Rural Municipality", NameNp = "दालोमे गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040605", DistrictCode = "D0406", DistrictName = "Mustang", Name = "Waragung Muktikshetra Rural Municipality", NameNp = "वरागुङ मुक्तिक्षेत्र गाउँपालिका" },

        // Myagdi District
        new MunicipalityViewModel { Code = "M040701", DistrictCode = "D0407", DistrictName = "Myagdi", Name = "Beni Municipality", NameNp = "बेनी नगरपालिका" },
        new MunicipalityViewModel { Code = "M040702", DistrictCode = "D0407", DistrictName = "Myagdi", Name = "Annapurna Rural Municipality", NameNp = "अन्नपूर्ण गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040703", DistrictCode = "D0407", DistrictName = "Myagdi", Name = "Dhaulagiri Rural Municipality", NameNp = "धौलागिरी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040704", DistrictCode = "D0407", DistrictName = "Myagdi", Name = "Malika Rural Municipality", NameNp = "मालिका गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040705", DistrictCode = "D0407", DistrictName = "Myagdi", Name = "Mangala Rural Municipality", NameNp = "मङ्गला गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040706", DistrictCode = "D0407", DistrictName = "Myagdi", Name = "Raghuganga Rural Municipality", NameNp = "रघुगंगा गाउँपालिका" },

        // Nawalpur District
        new MunicipalityViewModel { Code = "M040801", DistrictCode = "D0408", DistrictName = "Nawalpur", Name = "Kawasoti Municipality", NameNp = "कवासोती नगरपालिका" },
        new MunicipalityViewModel { Code = "M040802", DistrictCode = "D0408", DistrictName = "Nawalpur", Name = "Gaindakot Municipality", NameNp = "गैंडाकोट नगरपालिका" },
        new MunicipalityViewModel { Code = "M040803", DistrictCode = "D0408", DistrictName = "Nawalpur", Name = "Madhyabindu Municipality", NameNp = "मध्यविन्दु नगरपालिका" },
        new MunicipalityViewModel { Code = "M040804", DistrictCode = "D0408", DistrictName = "Nawalpur", Name = "Devchuli Municipality", NameNp = "देवचुली नगरपालिका" },
        new MunicipalityViewModel { Code = "M040805", DistrictCode = "D0408", DistrictName = "Nawalpur", Name = "Baudikali Rural Municipality", NameNp = "बौदीकाली गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040806", DistrictCode = "D0408", DistrictName = "Nawalpur", Name = "Bulingtar Rural Municipality", NameNp = "बुलिङटार गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040807", DistrictCode = "D0408", DistrictName = "Nawalpur", Name = "Binayi Tribeni Rural Municipality", NameNp = "बिनायी त्रिवेणी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040808", DistrictCode = "D0408", DistrictName = "Nawalpur", Name = "Hupsekot Rural Municipality", NameNp = "हुप्सेकोट गाउँपालिका" },

        // Parbat District
        new MunicipalityViewModel { Code = "M040901", DistrictCode = "D0409", DistrictName = "Parbat", Name = "Kushma Municipality", NameNp = "कुश्मा नगरपालिका" },
        new MunicipalityViewModel { Code = "M040902", DistrictCode = "D0409", DistrictName = "Parbat", Name = "Phalebas Municipality", NameNp = "फलेबास नगरपालिका" },
        new MunicipalityViewModel { Code = "M040903", DistrictCode = "D0409", DistrictName = "Parbat", Name = "Jaljala Rural Municipality", NameNp = "जलजला गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040904", DistrictCode = "D0409", DistrictName = "Parbat", Name = "Modi Rural Municipality", NameNp = "मोदी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040905", DistrictCode = "D0409", DistrictName = "Parbat", Name = "Paiyun Rural Municipality", NameNp = "पैयुँ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M040906", DistrictCode = "D0409", DistrictName = "Parbat", Name = "Bihadi Rural Municipality", NameNp = "बिहादी गाउँपालिका" },

        // Syangja District
        new MunicipalityViewModel { Code = "M041001", DistrictCode = "D0410", DistrictName = "Syangja", Name = "Waling Municipality", NameNp = "वालिङ नगरपालिका" },
        new MunicipalityViewModel { Code = "M041002", DistrictCode = "D0410", DistrictName = "Syangja", Name = "Putalibazar Municipality", NameNp = "पुतलीबजार नगरपालिका" },
        new MunicipalityViewModel { Code = "M041003", DistrictCode = "D0410", DistrictName = "Syangja", Name = "Galyang Municipality", NameNp = "गल्याङ नगरपालिका" },
        new MunicipalityViewModel { Code = "M041004", DistrictCode = "D0410", DistrictName = "Syangja", Name = "Chapakot Municipality", NameNp = "चापाकोट नगरपालिका" },
        new MunicipalityViewModel { Code = "M041005", DistrictCode = "D0410", DistrictName = "Syangja", Name = "Bhirkot Municipality", NameNp = "भिरकोट नगरपालिका" },
        new MunicipalityViewModel { Code = "M041006", DistrictCode = "D0410", DistrictName = "Syangja", Name = "Kaligandaki Rural Municipality", NameNp = "कालीगण्डकी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M041007", DistrictCode = "D0410", DistrictName = "Syangja", Name = "Arjunchaupari Rural Municipality", NameNp = "अर्जुनचौपारी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M041008", DistrictCode = "D0410", DistrictName = "Syangja", Name = "Aandhikhola Rural Municipality", NameNp = "आन्धीखोला गाउँपालिका" },
        new MunicipalityViewModel { Code = "M041009", DistrictCode = "D0410", DistrictName = "Syangja", Name = "Phedikhola Rural Municipality", NameNp = "फेदीखोला गाउँपालिका" },
        new MunicipalityViewModel { Code = "M041010", DistrictCode = "D0410", DistrictName = "Syangja", Name = "Harinas Rural Municipality", NameNp = "हरिनास गाउँपालिका" },

        // Tanahun District
        new MunicipalityViewModel { Code = "M041101", DistrictCode = "D0411", DistrictName = "Tanahun", Name = "Damauli Municipality", NameNp = "दमौली नगरपालिका" },
        new MunicipalityViewModel { Code = "M041102", DistrictCode = "D0411", DistrictName = "Tanahun", Name = "Shuklagandaki Municipality", NameNp = "शुक्लागण्डकी नगरपालिका" },
        new MunicipalityViewModel { Code = "M041103", DistrictCode = "D0411", DistrictName = "Tanahun", Name = "Bhanu Municipality", NameNp = "भानु नगरपालिका" },
        new MunicipalityViewModel { Code = "M041104", DistrictCode = "D0411", DistrictName = "Tanahun", Name = "Bhimad Municipality", NameNp = "भिमाद नगरपालिका" },
        new MunicipalityViewModel { Code = "M041105", DistrictCode = "D0411", DistrictName = "Tanahun", Name = "Rishing Rural Municipality", NameNp = "रिसिङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M041106", DistrictCode = "D0411", DistrictName = "Tanahun", Name = "Myagde Rural Municipality", NameNp = "म्याग्दे गाउँपालिका" },
        new MunicipalityViewModel { Code = "M041107", DistrictCode = "D0411", DistrictName = "Tanahun", Name = "Devghat Rural Municipality", NameNp = "देवघाट गाउँपालिका" },
        new MunicipalityViewModel { Code = "M041108", DistrictCode = "D0411", DistrictName = "Tanahun", Name = "Bandipur Rural Municipality", NameNp = "बन्दीपुर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M041109", DistrictCode = "D0411", DistrictName = "Tanahun", Name = "Ghiring Rural Municipality", NameNp = "घिरिङ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M041110", DistrictCode = "D0411", DistrictName = "Tanahun", Name = "Anbukhaireni Rural Municipality", NameNp = "अन्बुखैरेनी गाउँपालिका" },

        // Lumbini Province - Remaining Districts (Rupandehi already added)
        // Arghakhanchi District
        new MunicipalityViewModel { Code = "M050101", DistrictCode = "D0501", DistrictName = "Arghakhanchi", Name = "Sandhikharka Municipality", NameNp = "सन्धिखर्क नगरपालिका" },
        new MunicipalityViewModel { Code = "M050102", DistrictCode = "D0501", DistrictName = "Arghakhanchi", Name = "Bhumikasthan Municipality", NameNp = "भूमिकास्थान नगरपालिका" },
        new MunicipalityViewModel { Code = "M050103", DistrictCode = "D0501", DistrictName = "Arghakhanchi", Name = "Shitganga Municipality", NameNp = "शितगंगा नगरपालिका" },
        new MunicipalityViewModel { Code = "M050104", DistrictCode = "D0501", DistrictName = "Arghakhanchi", Name = "Chhatradev Rural Municipality", NameNp = "छत्रदेव गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050105", DistrictCode = "D0501", DistrictName = "Arghakhanchi", Name = "Panini Rural Municipality", NameNp = "पाणिनी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050106", DistrictCode = "D0501", DistrictName = "Arghakhanchi", Name = "Malyang Rural Municipality", NameNp = "मल्याङ गाउँपालिका" },

        // Banke District
        new MunicipalityViewModel { Code = "M050201", DistrictCode = "D0502", DistrictName = "Banke", Name = "Nepalgunj Sub-Metropolitan City", NameNp = "नेपालगञ्ज उपमहानगरपालिका" },
        new MunicipalityViewModel { Code = "M050202", DistrictCode = "D0502", DistrictName = "Banke", Name = "Kohalpur Municipality", NameNp = "कोहलपुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M050203", DistrictCode = "D0502", DistrictName = "Banke", Name = "Narainapur Rural Municipality", NameNp = "नरैनापुर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050204", DistrictCode = "D0502", DistrictName = "Banke", Name = "Rapti Sonari Rural Municipality", NameNp = "राप्ती सोनारी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050205", DistrictCode = "D0502", DistrictName = "Banke", Name = "Baijanath Rural Municipality", NameNp = "बैजनाथ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050206", DistrictCode = "D0502", DistrictName = "Banke", Name = "Duduwa Rural Municipality", NameNp = "दुडुवा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050207", DistrictCode = "D0502", DistrictName = "Banke", Name = "Janaki Rural Municipality", NameNp = "जानकी गाउँपालिका" },

        // Bardiya District
        new MunicipalityViewModel { Code = "M050301", DistrictCode = "D0503", DistrictName = "Bardiya", Name = "Gulariya Municipality", NameNp = "गुलरिया नगरपालिका" },
        new MunicipalityViewModel { Code = "M050302", DistrictCode = "D0503", DistrictName = "Bardiya", Name = "Rajapur Municipality", NameNp = "राजापुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M050303", DistrictCode = "D0503", DistrictName = "Bardiya", Name = "Madhuwan Municipality", NameNp = "मधुवन नगरपालिका" },
        new MunicipalityViewModel { Code = "M050304", DistrictCode = "D0503", DistrictName = "Bardiya", Name = "Thakurbaba Municipality", NameNp = "ठाकुरबाबा नगरपालिका" },
        new MunicipalityViewModel { Code = "M050305", DistrictCode = "D0503", DistrictName = "Bardiya", Name = "Badhaiyatal Rural Municipality", NameNp = "बधैयाताल गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050306", DistrictCode = "D0503", DistrictName = "Bardiya", Name = "Geruwa Rural Municipality", NameNp = "गेरुवा गाउँपालिका" },

        // Dang District
        new MunicipalityViewModel { Code = "M050401", DistrictCode = "D0504", DistrictName = "Dang", Name = "Ghorahi Sub-Metropolitan City", NameNp = "घोराही उपमहानगरपालिका" },
        new MunicipalityViewModel { Code = "M050402", DistrictCode = "D0504", DistrictName = "Dang", Name = "Tulsipur Sub-Metropolitan City", NameNp = "तुल्सीपुर उपमहानगरपालिका" },
        new MunicipalityViewModel { Code = "M050403", DistrictCode = "D0504", DistrictName = "Dang", Name = "Lamahi Municipality", NameNp = "लमही नगरपालिका" },
        new MunicipalityViewModel { Code = "M050404", DistrictCode = "D0504", DistrictName = "Dang", Name = "Shantinagar Rural Municipality", NameNp = "शान्तिनगर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050405", DistrictCode = "D0504", DistrictName = "Dang", Name = "Rapti Rural Municipality", NameNp = "राप्ती गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050406", DistrictCode = "D0504", DistrictName = "Dang", Name = "Gadhawa Rural Municipality", NameNp = "गढवा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050407", DistrictCode = "D0504", DistrictName = "Dang", Name = "Dangisharan Rural Municipality", NameNp = "दाङीशरण गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050408", DistrictCode = "D0504", DistrictName = "Dang", Name = "Babai Rural Municipality", NameNp = "बबई गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050409", DistrictCode = "D0504", DistrictName = "Dang", Name = "Rajpur Rural Municipality", NameNp = "राजपुर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050410", DistrictCode = "D0504", DistrictName = "Dang", Name = "Banglachuli Rural Municipality", NameNp = "बाङ्लाचुली गाउँपालिका" },

        // Gulmi District
        new MunicipalityViewModel { Code = "M050501", DistrictCode = "D0505", DistrictName = "Gulmi", Name = "Musikot Municipality", NameNp = "मुसिकोट नगरपालिका" },
        new MunicipalityViewModel { Code = "M050502", DistrictCode = "D0505", DistrictName = "Gulmi", Name = "Resunga Municipality", NameNp = "रेसुङ्गा नगरपालिका" },
        new MunicipalityViewModel { Code = "M050503", DistrictCode = "D0505", DistrictName = "Gulmi", Name = "Isma Rural Municipality", NameNp = "इस्मा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050504", DistrictCode = "D0505", DistrictName = "Gulmi", Name = "Kaligandaki Rural Municipality", NameNp = "कालीगण्डकी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050505", DistrictCode = "D0505", DistrictName = "Gulmi", Name = "Gulmidarbar Rural Municipality", NameNp = "गुल्मीदरबार गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050506", DistrictCode = "D0505", DistrictName = "Gulmi", Name = "Chandrakot Rural Municipality", NameNp = "चन्द्रकोट गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050507", DistrictCode = "D0505", DistrictName = "Gulmi", Name = "Madane Rural Municipality", NameNp = "मदाने गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050508", DistrictCode = "D0505", DistrictName = "Gulmi", Name = "Dhurkot Rural Municipality", NameNp = "ढुर्कोट गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050509", DistrictCode = "D0505", DistrictName = "Gulmi", Name = "Satyawati Rural Municipality", NameNp = "सत्यवती गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050510", DistrictCode = "D0505", DistrictName = "Gulmi", Name = "Ruru Rural Municipality", NameNp = "रुरु गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050511", DistrictCode = "D0505", DistrictName = "Gulmi", Name = "Malika Rural Municipality", NameNp = "मालिका गाउँपालिका" },

        // Kapilvastu District
        new MunicipalityViewModel { Code = "M050601", DistrictCode = "D0506", DistrictName = "Kapilvastu", Name = "Kapilvastu Municipality", NameNp = "कपिलवस्तु नगरपालिका" },
        new MunicipalityViewModel { Code = "M050602", DistrictCode = "D0506", DistrictName = "Kapilvastu", Name = "Buddhabhumi Municipality", NameNp = "बुद्धभूमि नगरपालिका" },
        new MunicipalityViewModel { Code = "M050603", DistrictCode = "D0506", DistrictName = "Kapilvastu", Name = "Shivaraj Municipality", NameNp = "शिवराज नगरपालिका" },
        new MunicipalityViewModel { Code = "M050604", DistrictCode = "D0506", DistrictName = "Kapilvastu", Name = "Maharajgunj Municipality", NameNp = "महाराजगञ्ज नगरपालिका" },
        new MunicipalityViewModel { Code = "M050605", DistrictCode = "D0506", DistrictName = "Kapilvastu", Name = "Banganga Municipality", NameNp = "बाणगंगा नगरपालिका" },
        new MunicipalityViewModel { Code = "M050606", DistrictCode = "D0506", DistrictName = "Kapilvastu", Name = "Krishnanagar Municipality", NameNp = "कृष्णनगर नगरपालिका" },
        new MunicipalityViewModel { Code = "M050607", DistrictCode = "D0506", DistrictName = "Kapilvastu", Name = "Yashodhara Rural Municipality", NameNp = "यशोधरा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050608", DistrictCode = "D0506", DistrictName = "Kapilvastu", Name = "Bijayanagar Rural Municipality", NameNp = "बिजयनगर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050609", DistrictCode = "D0506", DistrictName = "Kapilvastu", Name = "Mayadevi Rural Municipality", NameNp = "मायादेवी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050610", DistrictCode = "D0506", DistrictName = "Kapilvastu", Name = "Suddhodhan Rural Municipality", NameNp = "सुद्धोढन गाउँपालिका" },

        // Nawalparasi East District
        new MunicipalityViewModel { Code = "M050701", DistrictCode = "D0507", DistrictName = "Nawalparasi East", Name = "Gaidakot Municipality", NameNp = "गैडाकोट नगरपालिका" },
        new MunicipalityViewModel { Code = "M050702", DistrictCode = "D0507", DistrictName = "Nawalparasi East", Name = "Devchuli Municipality", NameNp = "देवचुली नगरपालिका" },
        new MunicipalityViewModel { Code = "M050703", DistrictCode = "D0507", DistrictName = "Nawalparasi East", Name = "Madhyabindu Municipality", NameNp = "मध्यविन्दु नगरपालिका" },
        new MunicipalityViewModel { Code = "M050704", DistrictCode = "D0507", DistrictName = "Nawalparasi East", Name = "Baudikali Rural Municipality", NameNp = "बौदीकाली गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050705", DistrictCode = "D0507", DistrictName = "Nawalparasi East", Name = "Bulingtar Rural Municipality", NameNp = "बुलिङटार गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050706", DistrictCode = "D0507", DistrictName = "Nawalparasi East", Name = "Binayi Tribeni Rural Municipality", NameNp = "बिनायी त्रिवेणी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050707", DistrictCode = "D0507", DistrictName = "Nawalparasi East", Name = "Hupsekot Rural Municipality", NameNp = "हुप्सेकोट गाउँपालिका" },

        // Palpa District
        new MunicipalityViewModel { Code = "M050801", DistrictCode = "D0508", DistrictName = "Palpa", Name = "Tansen Municipality", NameNp = "तानसेन नगरपालिका" },
        new MunicipalityViewModel { Code = "M050802", DistrictCode = "D0508", DistrictName = "Palpa", Name = "Rampur Municipality", NameNp = "रामपुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M050803", DistrictCode = "D0508", DistrictName = "Palpa", Name = "Rainadevi Chhahara Rural Municipality", NameNp = "रैनादेवी छहरा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050804", DistrictCode = "D0508", DistrictName = "Palpa", Name = "Ribdikot Rural Municipality", NameNp = "रिब्दीकोट गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050805", DistrictCode = "D0508", DistrictName = "Palpa", Name = "Mathagadhi Rural Municipality", NameNp = "माथागढी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050806", DistrictCode = "D0508", DistrictName = "Palpa", Name = "Nisdi Rural Municipality", NameNp = "निस्दी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050807", DistrictCode = "D0508", DistrictName = "Palpa", Name = "Bagnaskali Rural Municipality", NameNp = "बग्नासकाली गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050808", DistrictCode = "D0508", DistrictName = "Palpa", Name = "Tinau Rural Municipality", NameNp = "तिनाउ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050809", DistrictCode = "D0508", DistrictName = "Palpa", Name = "Rambha Rural Municipality", NameNp = "रम्भा गाउँपालिका" },

        // Pyuthan District
        new MunicipalityViewModel { Code = "M050901", DistrictCode = "D0509", DistrictName = "Pyuthan", Name = "Pyuthan Municipality", NameNp = "प्युठान नगरपालिका" },
        new MunicipalityViewModel { Code = "M050902", DistrictCode = "D0509", DistrictName = "Pyuthan", Name = "Swargadwari Municipality", NameNp = "स्वर्गद्वारी नगरपालिका" },
        new MunicipalityViewModel { Code = "M050903", DistrictCode = "D0509", DistrictName = "Pyuthan", Name = "Gaumukhi Rural Municipality", NameNp = "गौमुखी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050904", DistrictCode = "D0509", DistrictName = "Pyuthan", Name = "Jhimruk Rural Municipality", NameNp = "झिमरुक गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050905", DistrictCode = "D0509", DistrictName = "Pyuthan", Name = "Mallarani Rural Municipality", NameNp = "मल्लरानी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050906", DistrictCode = "D0509", DistrictName = "Pyuthan", Name = "Mand-vi Rural Municipality", NameNp = "माण्डवी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050907", DistrictCode = "D0509", DistrictName = "Pyuthan", Name = "Sarumarani Rural Municipality", NameNp = "सरुमरानी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M050908", DistrictCode = "D0509", DistrictName = "Pyuthan", Name = "Naubahini Rural Municipality", NameNp = "नौबहिनी गाउँपालिका" },

        // Rolpa District
        new MunicipalityViewModel { Code = "M051001", DistrictCode = "D0510", DistrictName = "Rolpa", Name = "Liwang Municipality", NameNp = "लिवाङ नगरपालिका" },
        new MunicipalityViewModel { Code = "M051002", DistrictCode = "D0510", DistrictName = "Rolpa", Name = "Runtigadhi Rural Municipality", NameNp = "रुन्टीगढी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M051003", DistrictCode = "D0510", DistrictName = "Rolpa", Name = "Sunchhahari Rural Municipality", NameNp = "सुन्छहरी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M051004", DistrictCode = "D0510", DistrictName = "Rolpa", Name = "Madi Rural Municipality", NameNp = "माडी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M051005", DistrictCode = "D0510", DistrictName = "Rolpa", Name = "Gangadev Rural Municipality", NameNp = "गंगादेव गाउँपालिका" },
        new MunicipalityViewModel { Code = "M051006", DistrictCode = "D0510", DistrictName = "Rolpa", Name = "Tribeni Rural Municipality", NameNp = "त्रिवेणी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M051007", DistrictCode = "D0510", DistrictName = "Rolpa", Name = "Pariwartan Rural Municipality", NameNp = "परिवर्तन गाउँपालिका" },
        new MunicipalityViewModel { Code = "M051008", DistrictCode = "D0510", DistrictName = "Rolpa", Name = "Lungri Rural Municipality", NameNp = "लुङ्ग्री गाउँपालिका" },
        new MunicipalityViewModel { Code = "M051009", DistrictCode = "D0510", DistrictName = "Rolpa", Name = "Sunil Smriti Rural Municipality", NameNp = "सुनिल स्मृति गाउँपालिका" },

        // Rukum East District
        new MunicipalityViewModel { Code = "M051101", DistrictCode = "D0511", DistrictName = "Rukum East", Name = "Rukumkot Municipality", NameNp = "रुकुमकोट नगरपालिका" },
        new MunicipalityViewModel { Code = "M051102", DistrictCode = "D0511", DistrictName = "Rukum East", Name = "Putha Uttarganga Rural Municipality", NameNp = "पुथा उत्तरगंगा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M051103", DistrictCode = "D0511", DistrictName = "Rukum East", Name = "Sisne Rural Municipality", NameNp = "सिस्ने गाउँपालिका" },

        // Rupandehi District (already added, but fixing code)
        new MunicipalityViewModel { Code = "M051201", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Butwal Sub-Metropolitan City", NameNp = "बुटवल उपमहानगरपालिका" },
        new MunicipalityViewModel { Code = "M051202", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Siddharthanagar Municipality", NameNp = "सिद्धार्थनगर नगरपालिका" },
        new MunicipalityViewModel { Code = "M051203", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Tilottama Municipality", NameNp = "तिलोत्तमा नगरपालिका" },
        new MunicipalityViewModel { Code = "M051204", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Lumbini Sanskritik Municipality", NameNp = "लुम्बिनी सांस्कृतिक नगरपालिका" },
        new MunicipalityViewModel { Code = "M051205", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Sainamaina Municipality", NameNp = "सैनामैना नगरपालिका" },
        new MunicipalityViewModel { Code = "M051206", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Devdaha Municipality", NameNp = "देवदह नगरपालिका" },
        new MunicipalityViewModel { Code = "M051207", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Marchawari Rural Municipality", NameNp = "मर्चवारी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M051208", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Kotahimai Rural Municipality", NameNp = "कोटहीमाई गाउँपालिका" },
        new MunicipalityViewModel { Code = "M051209", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Gaidahawa Rural Municipality", NameNp = "गैडाहवा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M051210", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Kanchan Rural Municipality", NameNp = "कञ्चन गाउँपालिका" },
        new MunicipalityViewModel { Code = "M051211", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Mayadevi Rural Municipality", NameNp = "मायादेवी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M051212", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Omsatiya Rural Municipality", NameNp = "ओमसतिया गाउँपालिका" },
        new MunicipalityViewModel { Code = "M051213", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Rohini Rural Municipality", NameNp = "रोहिणी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M051214", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Sammarimai Rural Municipality", NameNp = "सम्मरीमाई गाउँपालिका" },
        new MunicipalityViewModel { Code = "M051215", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Siyari Rural Municipality", NameNp = "सियारी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M051216", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Sudhdhodhan Rural Municipality", NameNp = "सुद्धोढन गाउँपालिका" },

        // Karnali Province
        // Dailekh District
        new MunicipalityViewModel { Code = "M060101", DistrictCode = "D0601", DistrictName = "Dailekh", Name = "Narayan Municipality", NameNp = "नारायण नगरपालिका" },
        new MunicipalityViewModel { Code = "M060102", DistrictCode = "D0601", DistrictName = "Dailekh", Name = "Dullu Municipality", NameNp = "दुल्लु नगरपालिका" },
        new MunicipalityViewModel { Code = "M060103", DistrictCode = "D0601", DistrictName = "Dailekh", Name = "Aathabis Municipality", NameNp = "आठबिस नगरपालिका" },
        new MunicipalityViewModel { Code = "M060104", DistrictCode = "D0601", DistrictName = "Dailekh", Name = "Chamunda Bindrasaini Municipality", NameNp = "चामुण्डा विन्द्रासैनी नगरपालिका" },
        new MunicipalityViewModel { Code = "M060105", DistrictCode = "D0601", DistrictName = "Dailekh", Name = "Thantikandh Rural Municipality", NameNp = "थान्तिकन्ध गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060106", DistrictCode = "D0601", DistrictName = "Dailekh", Name = "Bhairabi Rural Municipality", NameNp = "भैरवी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060107", DistrictCode = "D0601", DistrictName = "Dailekh", Name = "Dungeshwor Rural Municipality", NameNp = "डुङ्गेश्वर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060108", DistrictCode = "D0601", DistrictName = "Dailekh", Name = "Gurans Rural Municipality", NameNp = "गुराँस गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060109", DistrictCode = "D0601", DistrictName = "Dailekh", Name = "Naumule Rural Municipality", NameNp = "नौमुले गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060110", DistrictCode = "D0601", DistrictName = "Dailekh", Name = "Mahabu Rural Municipality", NameNp = "महाबु गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060111", DistrictCode = "D0601", DistrictName = "Dailekh", Name = "Bhagawatimai Rural Municipality", NameNp = "भगवतीमाई गाउँपालिका" },

        // Dolpa District
        new MunicipalityViewModel { Code = "M060201", DistrictCode = "D0602", DistrictName = "Dolpa", Name = "Thuli Bheri Municipality", NameNp = "ठूली भेरी नगरपालिका" },
        new MunicipalityViewModel { Code = "M060202", DistrictCode = "D0602", DistrictName = "Dolpa", Name = "Tripurasundari Municipality", NameNp = "त्रिपुरासुन्दरी नगरपालिका" },
        new MunicipalityViewModel { Code = "M060203", DistrictCode = "D0602", DistrictName = "Dolpa", Name = "Shey Phoksundo Rural Municipality", NameNp = "शे फोकसुन्डो गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060204", DistrictCode = "D0602", DistrictName = "Dolpa", Name = "Jagadulla Rural Municipality", NameNp = "जगदुल्ला गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060205", DistrictCode = "D0602", DistrictName = "Dolpa", Name = "Mudkechula Rural Municipality", NameNp = "मुड्केचुला गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060206", DistrictCode = "D0602", DistrictName = "Dolpa", Name = "Kaike Rural Municipality", NameNp = "काइके गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060207", DistrictCode = "D0602", DistrictName = "Dolpa", Name = "Dolpo Buddha Rural Municipality", NameNp = "डोल्पो बुद्ध गाउँपालिका" },

        // Humla District
        new MunicipalityViewModel { Code = "M060301", DistrictCode = "D0603", DistrictName = "Humla", Name = "Simikot Rural Municipality", NameNp = "सिमिकोट गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060302", DistrictCode = "D0603", DistrictName = "Humla", Name = "Namkha Rural Municipality", NameNp = "नाम्खा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060303", DistrictCode = "D0603", DistrictName = "Humla", Name = "Chankheli Rural Municipality", NameNp = "चाङ्खेली गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060304", DistrictCode = "D0603", DistrictName = "Humla", Name = "Kharpunath Rural Municipality", NameNp = "खर्पुनाथ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060305", DistrictCode = "D0603", DistrictName = "Humla", Name = "Sarkegad Rural Municipality", NameNp = "सर्केगाड गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060306", DistrictCode = "D0603", DistrictName = "Humla", Name = "Adanchuli Rural Municipality", NameNp = "अडान्चुली गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060307", DistrictCode = "D0603", DistrictName = "Humla", Name = "Tajakot Rural Municipality", NameNp = "ताजाकोट गाउँपालिका" },

        // Jajarkot District
        new MunicipalityViewModel { Code = "M060401", DistrictCode = "D0604", DistrictName = "Jajarkot", Name = "Chhedagad Municipality", NameNp = "छेडागाड नगरपालिका" },
        new MunicipalityViewModel { Code = "M060402", DistrictCode = "D0604", DistrictName = "Jajarkot", Name = "Bheri Municipality", NameNp = "भेरी नगरपालिका" },
        new MunicipalityViewModel { Code = "M060403", DistrictCode = "D0604", DistrictName = "Jajarkot", Name = "Barekot Rural Municipality", NameNp = "बरेकोट गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060404", DistrictCode = "D0604", DistrictName = "Jajarkot", Name = "Junichande Rural Municipality", NameNp = "जुनीचान्दे गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060405", DistrictCode = "D0604", DistrictName = "Jajarkot", Name = "Kuse Rural Municipality", NameNp = "कुसे गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060406", DistrictCode = "D0604", DistrictName = "Jajarkot", Name = "Shivalaya Rural Municipality", NameNp = "शिवालय गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060407", DistrictCode = "D0604", DistrictName = "Jajarkot", Name = "Nalagad Municipality", NameNp = "नालागाड नगरपालिका" },

        // Jumla District
        new MunicipalityViewModel { Code = "M060501", DistrictCode = "D0605", DistrictName = "Jumla", Name = "Chandannath Municipality", NameNp = "चन्दननाथ नगरपालिका" },
        new MunicipalityViewModel { Code = "M060502", DistrictCode = "D0605", DistrictName = "Jumla", Name = "Tila Rural Municipality", NameNp = "तिला गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060503", DistrictCode = "D0605", DistrictName = "Jumla", Name = "Guthichaur Rural Municipality", NameNp = "गुठिचौर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060504", DistrictCode = "D0605", DistrictName = "Jumla", Name = "Hima Rural Municipality", NameNp = "हिमा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060505", DistrictCode = "D0605", DistrictName = "Jumla", Name = "Kanakasundari Rural Municipality", NameNp = "कनकसुन्दरी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060506", DistrictCode = "D0605", DistrictName = "Jumla", Name = "Sinja Rural Municipality", NameNp = "सिञ्जा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060507", DistrictCode = "D0605", DistrictName = "Jumla", Name = "Tatopani Rural Municipality", NameNp = "तातोपानी गाउँपालिका" },

        // Kalikot District
        new MunicipalityViewModel { Code = "M060601", DistrictCode = "D0606", DistrictName = "Kalikot", Name = "Khandachakra Municipality", NameNp = "खण्डचक्र नगरपालिका" },
        new MunicipalityViewModel { Code = "M060602", DistrictCode = "D0606", DistrictName = "Kalikot", Name = "Raskot Municipality", NameNp = "रास्कोट नगरपालिका" },
        new MunicipalityViewModel { Code = "M060603", DistrictCode = "D0606", DistrictName = "Kalikot", Name = "Tilagufa Municipality", NameNp = "तिलागुफा नगरपालिका" },
        new MunicipalityViewModel { Code = "M060604", DistrictCode = "D0606", DistrictName = "Kalikot", Name = "Pachaljharana Rural Municipality", NameNp = "पचलझरना गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060605", DistrictCode = "D0606", DistrictName = "Kalikot", Name = "Sanni Triveni Rural Municipality", NameNp = "सान्नी त्रिवेणी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060606", DistrictCode = "D0606", DistrictName = "Kalikot", Name = "Naraharinath Rural Municipality", NameNp = "नरहरिनाथ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060607", DistrictCode = "D0606", DistrictName = "Kalikot", Name = "Mahawai Rural Municipality", NameNp = "महवै गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060608", DistrictCode = "D0606", DistrictName = "Kalikot", Name = "Subha Kalika Rural Municipality", NameNp = "शुभ कालिका गाउँपालिका" },

        // Mugu District
        new MunicipalityViewModel { Code = "M060701", DistrictCode = "D0607", DistrictName = "Mugu", Name = "Chhayanath Rara Municipality", NameNp = "छयानाथ रारा नगरपालिका" },
        new MunicipalityViewModel { Code = "M060702", DistrictCode = "D0607", DistrictName = "Mugu", Name = "Soru Rural Municipality", NameNp = "सोरु गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060703", DistrictCode = "D0607", DistrictName = "Mugu", Name = "Khatyad Rural Municipality", NameNp = "खत्याड गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060704", DistrictCode = "D0607", DistrictName = "Mugu", Name = "Mugum Karmarong Rural Municipality", NameNp = "मुगुम कर्मारोङ गाउँपालिका" },

        // Rukum West District
        new MunicipalityViewModel { Code = "M060801", DistrictCode = "D0608", DistrictName = "Rukum West", Name = "Musikot Municipality", NameNp = "मुसिकोट नगरपालिका" },
        new MunicipalityViewModel { Code = "M060802", DistrictCode = "D0608", DistrictName = "Rukum West", Name = "Chaurjahari Municipality", NameNp = "चौरजहारी नगरपालिका" },
        new MunicipalityViewModel { Code = "M060803", DistrictCode = "D0608", DistrictName = "Rukum West", Name = "Aathbiskot Municipality", NameNp = "आठबिसकोट नगरपालिका" },
        new MunicipalityViewModel { Code = "M060804", DistrictCode = "D0608", DistrictName = "Rukum West", Name = "Banfikot Rural Municipality", NameNp = "बान्फीकोट गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060805", DistrictCode = "D0608", DistrictName = "Rukum West", Name = "Sisne Rural Municipality", NameNp = "सिस्ने गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060806", DistrictCode = "D0608", DistrictName = "Rukum West", Name = "Triveni Rural Municipality", NameNp = "त्रिवेणी गाउँपालिका" },

        // Salyan District
        new MunicipalityViewModel { Code = "M060901", DistrictCode = "D0609", DistrictName = "Salyan", Name = "Shaarda Municipality", NameNp = "शारदा नगरपालिका" },
        new MunicipalityViewModel { Code = "M060902", DistrictCode = "D0609", DistrictName = "Salyan", Name = "Bagchaur Municipality", NameNp = "बागचौर नगरपालिका" },
        new MunicipalityViewModel { Code = "M060903", DistrictCode = "D0609", DistrictName = "Salyan", Name = "Bangad Kupinde Municipality", NameNp = "बाङ्गाद कुपिण्डे नगरपालिका" },
        new MunicipalityViewModel { Code = "M060904", DistrictCode = "D0609", DistrictName = "Salyan", Name = "Chhatreshwori Rural Municipality", NameNp = "छत्रेश्वरी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060905", DistrictCode = "D0609", DistrictName = "Salyan", Name = "Darma Rural Municipality", NameNp = "दर्मा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060906", DistrictCode = "D0609", DistrictName = "Salyan", Name = "Dhorchaur Rural Municipality", NameNp = "ढोर्चौर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060907", DistrictCode = "D0609", DistrictName = "Salyan", Name = "Kalimati Rural Municipality", NameNp = "कालिमाटी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060908", DistrictCode = "D0609", DistrictName = "Salyan", Name = "Kapurkot Rural Municipality", NameNp = "कपुरकोट गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060909", DistrictCode = "D0609", DistrictName = "Salyan", Name = "Kumakh Rural Municipality", NameNp = "कुमाख गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060910", DistrictCode = "D0609", DistrictName = "Salyan", Name = "Siddha Kumakh Rural Municipality", NameNp = "सिद्ध कुमाख गाउँपालिका" },
        new MunicipalityViewModel { Code = "M060911", DistrictCode = "D0609", DistrictName = "Salyan", Name = "Tribeni Rural Municipality", NameNp = "त्रिवेणी गाउँपालिका" },

        // Surkhet District
        new MunicipalityViewModel { Code = "M061001", DistrictCode = "D0610", DistrictName = "Surkhet", Name = "Birendranagar Municipality", NameNp = "वीरेन्द्रनगर नगरपालिका" },
        new MunicipalityViewModel { Code = "M061002", DistrictCode = "D0610", DistrictName = "Surkhet", Name = "Bheriganga Municipality", NameNp = "भेरीगंगा नगरपालिका" },
        new MunicipalityViewModel { Code = "M061003", DistrictCode = "D0610", DistrictName = "Surkhet", Name = "Gurbhakot Municipality", NameNp = "गुर्भाकोट नगरपालिका" },
        new MunicipalityViewModel { Code = "M061004", DistrictCode = "D0610", DistrictName = "Surkhet", Name = "Panchpuri Municipality", NameNp = "पञ्चपुरी नगरपालिका" },
        new MunicipalityViewModel { Code = "M061005", DistrictCode = "D0610", DistrictName = "Surkhet", Name = "Lekbeshi Municipality", NameNp = "लेकबेशी नगरपालिका" },
        new MunicipalityViewModel { Code = "M061006", DistrictCode = "D0610", DistrictName = "Surkhet", Name = "Barahatal Rural Municipality", NameNp = "बराहताल गाउँपालिका" },
        new MunicipalityViewModel { Code = "M061007", DistrictCode = "D0610", DistrictName = "Surkhet", Name = "Chaukune Rural Municipality", NameNp = "चौकुने गाउँपालिका" },
        new MunicipalityViewModel { Code = "M061008", DistrictCode = "D0610", DistrictName = "Surkhet", Name = "Chingad Rural Municipality", NameNp = "चिङ्गाड गाउँपालिका" },
        new MunicipalityViewModel { Code = "M061009", DistrictCode = "D0610", DistrictName = "Surkhet", Name = "Simta Rural Municipality", NameNp = "सिम्ता गाउँपालिका" },

        // Sudurpashchim Province
        // Achham District
        new MunicipalityViewModel { Code = "M070101", DistrictCode = "D0701", DistrictName = "Achham", Name = "Mangalsen Municipality", NameNp = "मंगलसेन नगरपालिका" },
        new MunicipalityViewModel { Code = "M070102", DistrictCode = "D0701", DistrictName = "Achham", Name = "Sanphebagar Municipality", NameNp = "साफेबगर नगरपालिका" },
        new MunicipalityViewModel { Code = "M070103", DistrictCode = "D0701", DistrictName = "Achham", Name = "Kamalbazar Municipality", NameNp = "कमलबजार नगरपालिका" },
        new MunicipalityViewModel { Code = "M070104", DistrictCode = "D0701", DistrictName = "Achham", Name = "Panchadewal Binayak Municipality", NameNp = "पञ्चदेवल बिनायक नगरपालिका" },
        new MunicipalityViewModel { Code = "M070105", DistrictCode = "D0701", DistrictName = "Achham", Name = "Ramaroshan Rural Municipality", NameNp = "रामरोशन गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070106", DistrictCode = "D0701", DistrictName = "Achham", Name = "Chaurpati Rural Municipality", NameNp = "चौरपाटी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070107", DistrictCode = "D0701", DistrictName = "Achham", Name = "Turmakhad Rural Municipality", NameNp = "तुर्माखाड गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070108", DistrictCode = "D0701", DistrictName = "Achham", Name = "Mellekh Rural Municipality", NameNp = "मेलेख गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070109", DistrictCode = "D0701", DistrictName = "Achham", Name = "Dhakari Rural Municipality", NameNp = "ढकारी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070110", DistrictCode = "D0701", DistrictName = "Achham", Name = "Bannigadhi Jayagadh Rural Municipality", NameNp = "बन्नीगढी जयगढ गाउँपालिका" },

        // Baitadi District
        new MunicipalityViewModel { Code = "M070201", DistrictCode = "D0702", DistrictName = "Baitadi", Name = "Dasharathchand Municipality", NameNp = "दशरथचन्द नगरपालिका" },
        new MunicipalityViewModel { Code = "M070202", DistrictCode = "D0702", DistrictName = "Baitadi", Name = "Patan Municipality", NameNp = "पाटन नगरपालिका" },
        new MunicipalityViewModel { Code = "M070203", DistrictCode = "D0702", DistrictName = "Baitadi", Name = "Melauli Municipality", NameNp = "मेलौली नगरपालिका" },
        new MunicipalityViewModel { Code = "M070204", DistrictCode = "D0702", DistrictName = "Baitadi", Name = "Purchaudi Municipality", NameNp = "पurchौडी नगरपालिका" },
        new MunicipalityViewModel { Code = "M070205", DistrictCode = "D0702", DistrictName = "Baitadi", Name = "Sigas Rural Municipality", NameNp = "सिगास गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070206", DistrictCode = "D0702", DistrictName = "Baitadi", Name = "Shivanath Rural Municipality", NameNp = "शिवनाथ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070207", DistrictCode = "D0702", DistrictName = "Baitadi", Name = "Surnaya Rural Municipality", NameNp = "सुर्नया गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070208", DistrictCode = "D0702", DistrictName = "Baitadi", Name = "Dilasaini Rural Municipality", NameNp = "दिलासैनी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070209", DistrictCode = "D0702", DistrictName = "Baitadi", Name = "Dogdakedar Rural Municipality", NameNp = "डोगडाकेदार गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070210", DistrictCode = "D0702", DistrictName = "Baitadi", Name = "Durgathali Rural Municipality", NameNp = "दुर्गाथली गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070211", DistrictCode = "D0702", DistrictName = "Baitadi", Name = "Pancheshwar Rural Municipality", NameNp = "पञ्चेश्वर गाउँपालिका" },

        // Bajhang District
        new MunicipalityViewModel { Code = "M070301", DistrictCode = "D0703", DistrictName = "Bajhang", Name = "Jaya Prithvi Municipality", NameNp = "जय पृथ्वी नगरपालिका" },
        new MunicipalityViewModel { Code = "M070302", DistrictCode = "D0703", DistrictName = "Bajhang", Name = "Bungal Municipality", NameNp = "बुङ्गल नगरपालिका" },
        new MunicipalityViewModel { Code = "M070303", DistrictCode = "D0703", DistrictName = "Bajhang", Name = "Khaptadchhanna Rural Municipality", NameNp = "खप्तडछान्ना गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070304", DistrictCode = "D0703", DistrictName = "Bajhang", Name = "Thalara Rural Municipality", NameNp = "थलारा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070305", DistrictCode = "D0703", DistrictName = "Bajhang", Name = "Bitthadchir Rural Municipality", NameNp = "बित्थाडचिर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070306", DistrictCode = "D0703", DistrictName = "Bajhang", Name = "Chhabis Pathibhera Rural Municipality", NameNp = "छबिस पाथीभेरा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070307", DistrictCode = "D0703", DistrictName = "Bajhang", Name = "Durgathali Rural Municipality", NameNp = "दुर्गाथली गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070308", DistrictCode = "D0703", DistrictName = "Bajhang", Name = "Kedarseu Rural Municipality", NameNp = "केदारसेउ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070309", DistrictCode = "D0703", DistrictName = "Bajhang", Name = "Masta Rural Municipality", NameNp = "मस्ता गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070310", DistrictCode = "D0703", DistrictName = "Bajhang", Name = "Surma Rural Municipality", NameNp = "सुर्मा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070311", DistrictCode = "D0703", DistrictName = "Bajhang", Name = "Talkot Rural Municipality", NameNp = "तालकोट गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070312", DistrictCode = "D0703", DistrictName = "Bajhang", Name = "Sai Rural Municipality", NameNp = "साई गाउँपालिका" },

        // Bajura District
        new MunicipalityViewModel { Code = "M070401", DistrictCode = "D0704", DistrictName = "Bajura", Name = "Badimalika Municipality", NameNp = "बडीमालिका नगरपालिका" },
        new MunicipalityViewModel { Code = "M070402", DistrictCode = "D0704", DistrictName = "Bajura", Name = "Tribeni Municipality", NameNp = "त्रिवेणी नगरपालिका" },
        new MunicipalityViewModel { Code = "M070403", DistrictCode = "D0704", DistrictName = "Bajura", Name = "Budhiganga Municipality", NameNp = "बुढीगंगा नगरपालिका" },
        new MunicipalityViewModel { Code = "M070404", DistrictCode = "D0704", DistrictName = "Bajura", Name = "Budhinanda Municipality", NameNp = "बुढीनन्दा नगरपालिका" },
        new MunicipalityViewModel { Code = "M070405", DistrictCode = "D0704", DistrictName = "Bajura", Name = "Gaumul Rural Municipality", NameNp = "गौमुल गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070406", DistrictCode = "D0704", DistrictName = "Bajura", Name = "Himali Rural Municipality", NameNp = "हिमाली गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070407", DistrictCode = "D0704", DistrictName = "Bajura", Name = "Jagannath Rural Municipality", NameNp = "जगन्नाथ गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070408", DistrictCode = "D0704", DistrictName = "Bajura", Name = "Swami Kartik Khapar Rural Municipality", NameNp = "स्वामी कार्तिक खापर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070409", DistrictCode = "D0704", DistrictName = "Bajura", Name = "Pandav Gupha Rural Municipality", NameNp = "पाण्डव गुफा गाउँपालिका" },

        // Dadeldhura District
        new MunicipalityViewModel { Code = "M070501", DistrictCode = "D0705", DistrictName = "Dadeldhura", Name = "Amargadhi Municipality", NameNp = "अमरगढी नगरपालिका" },
        new MunicipalityViewModel { Code = "M070502", DistrictCode = "D0705", DistrictName = "Dadeldhura", Name = "Parshuram Municipality", NameNp = "परशुराम नगरपालिका" },
        new MunicipalityViewModel { Code = "M070503", DistrictCode = "D0705", DistrictName = "Dadeldhura", Name = "Aalital Rural Municipality", NameNp = "आलिताल गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070504", DistrictCode = "D0705", DistrictName = "Dadeldhura", Name = "Bhageshwar Rural Municipality", NameNp = "भागेश्वर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070505", DistrictCode = "D0705", DistrictName = "Dadeldhura", Name = "Navadurga Rural Municipality", NameNp = "नवदुर्गा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070506", DistrictCode = "D0705", DistrictName = "Dadeldhura", Name = "Ajaymeru Rural Municipality", NameNp = "अजयमेरु गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070507", DistrictCode = "D0705", DistrictName = "Dadeldhura", Name = "Ganyapadhura Rural Municipality", NameNp = "गन्यापधुरा गाउँपालिका" },

        // Darchula District
        new MunicipalityViewModel { Code = "M070601", DistrictCode = "D0706", DistrictName = "Darchula", Name = "Mahakali Municipality", NameNp = "महाकाली नगरपालिका" },
        new MunicipalityViewModel { Code = "M070602", DistrictCode = "D0706", DistrictName = "Darchula", Name = "Shailyashikhar Municipality", NameNp = "शैल्यशिखर नगरपालिका" },
        new MunicipalityViewModel { Code = "M070603", DistrictCode = "D0706", DistrictName = "Darchula", Name = "Naugad Rural Municipality", NameNp = "नौगाड गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070604", DistrictCode = "D0706", DistrictName = "Darchula", Name = "Marma Rural Municipality", NameNp = "मर्मा गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070605", DistrictCode = "D0706", DistrictName = "Darchula", Name = "Duhun Rural Municipality", NameNp = "दुहुन गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070606", DistrictCode = "D0706", DistrictName = "Darchula", Name = "Lekam Rural Municipality", NameNp = "लेकाम गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070607", DistrictCode = "D0706", DistrictName = "Darchula", Name = "Byas Rural Municipality", NameNp = "ब्यास गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070608", DistrictCode = "D0706", DistrictName = "Darchula", Name = "Apihimal Rural Municipality", NameNp = "अपिहिमाल गाउँपालिका" },

        // Doti District
        new MunicipalityViewModel { Code = "M070701", DistrictCode = "D0707", DistrictName = "Doti", Name = "Dipayal Silgadhi Municipality", NameNp = "दिपायल सिलगढी नगरपालिका" },
        new MunicipalityViewModel { Code = "M070702", DistrictCode = "D0707", DistrictName = "Doti", Name = "Shikhar Municipality", NameNp = "शिखर नगरपालिका" },
        new MunicipalityViewModel { Code = "M070703", DistrictCode = "D0707", DistrictName = "Doti", Name = "Purbichauki Rural Municipality", NameNp = "पूर्विचौकी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070704", DistrictCode = "D0707", DistrictName = "Doti", Name = "Jorayal Rural Municipality", NameNp = "जोरायल गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070705", DistrictCode = "D0707", DistrictName = "Doti", Name = "Sayal Rural Municipality", NameNp = "सयाल गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070706", DistrictCode = "D0707", DistrictName = "Doti", Name = "Aadarsha Rural Municipality", NameNp = "आदर्श गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070707", DistrictCode = "D0707", DistrictName = "Doti", Name = "K I Singh Rural Municipality", NameNp = "के आई सिंह गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070708", DistrictCode = "D0707", DistrictName = "Doti", Name = "Bogatan Rural Municipality", NameNp = "बोगटान गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070709", DistrictCode = "D0707", DistrictName = "Doti", Name = "Badikedar Rural Municipality", NameNp = "बडीकेदार गाउँपालिका" },

        // Kailali District
        new MunicipalityViewModel { Code = "M070801", DistrictCode = "D0708", DistrictName = "Kailali", Name = "Dhangadhi Sub-Metropolitan City", NameNp = "धनगढी उपमहानगरपालिका" },
        new MunicipalityViewModel { Code = "M070802", DistrictCode = "D0708", DistrictName = "Kailali", Name = "Tikapur Municipality", NameNp = "टिकापुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M070803", DistrictCode = "D0708", DistrictName = "Kailali", Name = "Ghodaghodi Municipality", NameNp = "घोडाघोडी नगरपालिका" },
        new MunicipalityViewModel { Code = "M070804", DistrictCode = "D0708", DistrictName = "Kailali", Name = "Lamkichuha Municipality", NameNp = "लम्कीचुहा नगरपालिका" },
        new MunicipalityViewModel { Code = "M070805", DistrictCode = "D0708", DistrictName = "Kailali", Name = "Bhajani Municipality", NameNp = "भजनी नगरपालिका" },
        new MunicipalityViewModel { Code = "M070806", DistrictCode = "D0708", DistrictName = "Kailali", Name = "Godawari Municipality", NameNp = "गोदावरी नगरपालिका" },
        new MunicipalityViewModel { Code = "M070807", DistrictCode = "D0708", DistrictName = "Kailali", Name = "Gauriganga Municipality", NameNp = "गौरीगंगा नगरपालिका" },
        new MunicipalityViewModel { Code = "M070808", DistrictCode = "D0708", DistrictName = "Kailali", Name = "Janaki Rural Municipality", NameNp = "जानकी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070809", DistrictCode = "D0708", DistrictName = "Kailali", Name = "Kailari Rural Municipality", NameNp = "कैलारी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070810", DistrictCode = "D0708", DistrictName = "Kailali", Name = "Joshipur Rural Municipality", NameNp = "जोशीपुर गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070811", DistrictCode = "D0708", DistrictName = "Kailali", Name = "Bardagoriya Rural Municipality", NameNp = "बर्दगोरिया गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070812", DistrictCode = "D0708", DistrictName = "Kailali", Name = "Mohanyal Rural Municipality", NameNp = "मोहन्याल गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070813", DistrictCode = "D0708", DistrictName = "Kailali", Name = "Chure Rural Municipality", NameNp = "चुरे गाउँपालिका" },

        // Kanchanpur District
        new MunicipalityViewModel { Code = "M070901", DistrictCode = "D0709", DistrictName = "Kanchanpur", Name = "Bhimdatta Municipality", NameNp = "भीमदत्त नगरपालिका" },
        new MunicipalityViewModel { Code = "M070902", DistrictCode = "D0709", DistrictName = "Kanchanpur", Name = "Punarbas Municipality", NameNp = "पुनर्वास नगरपालिका" },
        new MunicipalityViewModel { Code = "M070903", DistrictCode = "D0709", DistrictName = "Kanchanpur", Name = "Bedkot Municipality", NameNp = "बेडकोट नगरपालिका" },
        new MunicipalityViewModel { Code = "M070904", DistrictCode = "D0709", DistrictName = "Kanchanpur", Name = "Mahakali Municipality", NameNp = "महाकाली नगरपालिका" },
        new MunicipalityViewModel { Code = "M070905", DistrictCode = "D0709", DistrictName = "Kanchanpur", Name = "Shuklaphanta Municipality", NameNp = "शुक्लाफान्ता नगरपालिका" },
        new MunicipalityViewModel { Code = "M070906", DistrictCode = "D0709", DistrictName = "Kanchanpur", Name = "Belauri Municipality", NameNp = "बेलौरी नगरपालिका" },
        new MunicipalityViewModel { Code = "M070907", DistrictCode = "D0709", DistrictName = "Kanchanpur", Name = "Krishnapur Municipality", NameNp = "कृष्णपुर नगरपालिका" },
        new MunicipalityViewModel { Code = "M070908", DistrictCode = "D0709", DistrictName = "Kanchanpur", Name = "Laljhadi Rural Municipality", NameNp = "लालझाडी गाउँपालिका" },
        new MunicipalityViewModel { Code = "M070909", DistrictCode = "D0709", DistrictName = "Kanchanpur", Name = "Dodhara Chandani Rural Municipality", NameNp = "दोढारा चन्दनी गाउँपालिका" }
    };

    private static readonly List<WardViewModel> Wards = new()
    {
        // Kathmandu Metropolitan City Wards
        new WardViewModel { Code = "W03050101", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 1, NameNp = "वडा १" },
        new WardViewModel { Code = "W03050102", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 2, NameNp = "वडा २" },
        new WardViewModel { Code = "W03050103", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 3, NameNp = "वडा ३" },
        new WardViewModel { Code = "W03050104", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 4, NameNp = "वडा ४" },
        new WardViewModel { Code = "W03050105", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 5, NameNp = "वडा ५" },
        new WardViewModel { Code = "W03050106", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 6, NameNp = "वडा ६" },
        new WardViewModel { Code = "W03050107", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 7, NameNp = "वडा ७" },
        new WardViewModel { Code = "W03050108", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 8, NameNp = "वडा ८" },
        new WardViewModel { Code = "W03050109", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 9, NameNp = "वडा ९" },
        new WardViewModel { Code = "W03050110", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 10, NameNp = "वडा १०" },
        new WardViewModel { Code = "W03050111", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 11, NameNp = "वडा ११" },
        new WardViewModel { Code = "W03050112", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 12, NameNp = "वडा १२" },
        new WardViewModel { Code = "W03050113", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 13, NameNp = "वडा १३" },
        new WardViewModel { Code = "W03050114", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 14, NameNp = "वडा १४" },
        new WardViewModel { Code = "W03050115", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 15, NameNp = "वडा १५" },
        new WardViewModel { Code = "W03050116", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 16, NameNp = "वडा १६" },
        new WardViewModel { Code = "W03050117", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 17, NameNp = "वडा १७" },
        new WardViewModel { Code = "W03050118", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 18, NameNp = "वडा १८" },
        new WardViewModel { Code = "W03050119", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 19, NameNp = "वडा १९" },
        new WardViewModel { Code = "W03050120", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 20, NameNp = "वडा २०" },
        new WardViewModel { Code = "W03050121", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 21, NameNp = "वडा २१" },
        new WardViewModel { Code = "W03050122", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 22, NameNp = "वडा २२" },
        new WardViewModel { Code = "W03050123", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 23, NameNp = "वडा २३" },
        new WardViewModel { Code = "W03050124", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 24, NameNp = "वडा २४" },
        new WardViewModel { Code = "W03050125", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 25, NameNp = "वडा २५" },
        new WardViewModel { Code = "W03050126", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 26, NameNp = "वडा २६" },
        new WardViewModel { Code = "W03050127", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 27, NameNp = "वडा २७" },
        new WardViewModel { Code = "W03050128", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 28, NameNp = "वडा २८" },
        new WardViewModel { Code = "W03050129", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 29, NameNp = "वडा २९" },
        new WardViewModel { Code = "W03050130", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 30, NameNp = "वडा ३०" },
        new WardViewModel { Code = "W03050131", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 31, NameNp = "वडा ३१" },
        new WardViewModel { Code = "W03050132", MunicipalityCode = "M030501", MunicipalityName = "Kathmandu Metropolitan City", WardNumber = 32, NameNp = "वडा ३२" },

        // Lalitpur Metropolitan City Wards
        new WardViewModel { Code = "W03070101", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 1, NameNp = "वडा १" },
        new WardViewModel { Code = "W03070102", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 2, NameNp = "वडा २" },
        new WardViewModel { Code = "W03070103", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 3, NameNp = "वडा ३" },
        new WardViewModel { Code = "W03070104", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 4, NameNp = "वडा ४" },
        new WardViewModel { Code = "W03070105", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 5, NameNp = "वडा ५" },
        new WardViewModel { Code = "W03070106", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 6, NameNp = "वडा ६" },
        new WardViewModel { Code = "W03070107", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 7, NameNp = "वडा ७" },
        new WardViewModel { Code = "W03070108", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 8, NameNp = "वडा ८" },
        new WardViewModel { Code = "W03070109", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 9, NameNp = "वडा ९" },
        new WardViewModel { Code = "W03070110", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 10, NameNp = "वडा १०" },
        new WardViewModel { Code = "W03070111", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 11, NameNp = "वडा ११" },
        new WardViewModel { Code = "W03070112", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 12, NameNp = "वडा १२" },
        new WardViewModel { Code = "W03070113", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 13, NameNp = "वडा १३" },
        new WardViewModel { Code = "W03070114", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 14, NameNp = "वडा १४" },
        new WardViewModel { Code = "W03070115", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 15, NameNp = "वडा १५" },
        new WardViewModel { Code = "W03070116", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 16, NameNp = "वडा १६" },
        new WardViewModel { Code = "W03070117", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 17, NameNp = "वडा १७" },
        new WardViewModel { Code = "W03070118", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 18, NameNp = "वडा १८" },
        new WardViewModel { Code = "W03070119", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 19, NameNp = "वडा १९" },
        new WardViewModel { Code = "W03070120", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 20, NameNp = "वडा २०" },
        new WardViewModel { Code = "W03070121", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 21, NameNp = "वडा २१" },
        new WardViewModel { Code = "W03070122", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 22, NameNp = "वडा २२" },
        new WardViewModel { Code = "W03070123", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 23, NameNp = "वडा २३" },
        new WardViewModel { Code = "W03070124", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 24, NameNp = "वडा २४" },
        new WardViewModel { Code = "W03070125", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 25, NameNp = "वडा २५" },
        new WardViewModel { Code = "W03070126", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 26, NameNp = "वडा २६" },
        new WardViewModel { Code = "W03070127", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 27, NameNp = "वडा २७" },
        new WardViewModel { Code = "W03070128", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 28, NameNp = "वडा २८" },
        new WardViewModel { Code = "W03070129", MunicipalityCode = "M030701", MunicipalityName = "Lalitpur Metropolitan City", WardNumber = 29, NameNp = "वडा २९" },

        // Bhaktapur Municipality Wards
        new WardViewModel { Code = "W03010101", MunicipalityCode = "M030101", MunicipalityName = "Bhaktapur Municipality", WardNumber = 1, NameNp = "वडा १" },
        new WardViewModel { Code = "W03010102", MunicipalityCode = "M030101", MunicipalityName = "Bhaktapur Municipality", WardNumber = 2, NameNp = "वडा २" },
        new WardViewModel { Code = "W03010103", MunicipalityCode = "M030101", MunicipalityName = "Bhaktapur Municipality", WardNumber = 3, NameNp = "वडा ३" },
        new WardViewModel { Code = "W03010104", MunicipalityCode = "M030101", MunicipalityName = "Bhaktapur Municipality", WardNumber = 4, NameNp = "वडा ४" },
        new WardViewModel { Code = "W03010105", MunicipalityCode = "M030101", MunicipalityName = "Bhaktapur Municipality", WardNumber = 5, NameNp = "वडा ५" },
        new WardViewModel { Code = "W03010106", MunicipalityCode = "M030101", MunicipalityName = "Bhaktapur Municipality", WardNumber = 6, NameNp = "वडा ६" },
        new WardViewModel { Code = "W03010107", MunicipalityCode = "M030101", MunicipalityName = "Bhaktapur Municipality", WardNumber = 7, NameNp = "वडा ७" },
        new WardViewModel { Code = "W03010108", MunicipalityCode = "M030101", MunicipalityName = "Bhaktapur Municipality", WardNumber = 8, NameNp = "वडा ८" },
        new WardViewModel { Code = "W03010109", MunicipalityCode = "M030101", MunicipalityName = "Bhaktapur Municipality", WardNumber = 9, NameNp = "वडा ९" },
        new WardViewModel { Code = "W03010110", MunicipalityCode = "M030101", MunicipalityName = "Bhaktapur Municipality", WardNumber = 10, NameNp = "वडा १०" },

        // Bharatpur Metropolitan City Wards
        new WardViewModel { Code = "W03020101", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 1, NameNp = "वडा १" },
        new WardViewModel { Code = "W03020102", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 2, NameNp = "वडा २" },
        new WardViewModel { Code = "W03020103", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 3, NameNp = "वडा ३" },
        new WardViewModel { Code = "W03020104", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 4, NameNp = "वडा ४" },
        new WardViewModel { Code = "W03020105", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 5, NameNp = "वडा ५" },
        new WardViewModel { Code = "W03020106", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 6, NameNp = "वडा ६" },
        new WardViewModel { Code = "W03020107", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 7, NameNp = "वडा ७" },
        new WardViewModel { Code = "W03020108", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 8, NameNp = "वडा ८" },
        new WardViewModel { Code = "W03020109", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 9, NameNp = "वडा ९" },
        new WardViewModel { Code = "W03020110", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 10, NameNp = "वडा १०" },
        new WardViewModel { Code = "W03020111", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 11, NameNp = "वडा ११" },
        new WardViewModel { Code = "W03020112", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 12, NameNp = "वडा १२" },
        new WardViewModel { Code = "W03020113", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 13, NameNp = "वडा १३" },
        new WardViewModel { Code = "W03020114", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 14, NameNp = "वडा १४" },
        new WardViewModel { Code = "W03020115", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 15, NameNp = "वडा १५" },
        new WardViewModel { Code = "W03020116", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 16, NameNp = "वडा १६" },
        new WardViewModel { Code = "W03020117", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 17, NameNp = "वडा १७" },
        new WardViewModel { Code = "W03020118", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 18, NameNp = "वडा १८" },
        new WardViewModel { Code = "W03020119", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 19, NameNp = "वडा १९" },
        new WardViewModel { Code = "W03020120", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 20, NameNp = "वडा २०" },
        new WardViewModel { Code = "W03020121", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 21, NameNp = "वडा २१" },
        new WardViewModel { Code = "W03020122", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 22, NameNp = "वडा २२" },
        new WardViewModel { Code = "W03020123", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 23, NameNp = "वडा २३" },
        new WardViewModel { Code = "W03020124", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 24, NameNp = "वडा २४" },
        new WardViewModel { Code = "W03020125", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 25, NameNp = "वडा २५" },
        new WardViewModel { Code = "W03020126", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 26, NameNp = "वडा २६" },
        new WardViewModel { Code = "W03020127", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 27, NameNp = "वडा २७" },
        new WardViewModel { Code = "W03020128", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 28, NameNp = "वडा २८" },
        new WardViewModel { Code = "W03020129", MunicipalityCode = "M030201", MunicipalityName = "Bharatpur Metropolitan City", WardNumber = 29, NameNp = "वडा २९" },

        // Pokhara Metropolitan City Wards
        new WardViewModel { Code = "W04030101", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 1, NameNp = "वडा १" },
        new WardViewModel { Code = "W04030102", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 2, NameNp = "वडा २" },
        new WardViewModel { Code = "W04030103", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 3, NameNp = "वडा ३" },
        new WardViewModel { Code = "W04030104", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 4, NameNp = "वडा ४" },
        new WardViewModel { Code = "W04030105", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 5, NameNp = "वडा ५" },
        new WardViewModel { Code = "W04030106", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 6, NameNp = "वडा ६" },
        new WardViewModel { Code = "W04030107", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 7, NameNp = "वडा ७" },
        new WardViewModel { Code = "W04030108", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 8, NameNp = "वडा ८" },
        new WardViewModel { Code = "W04030109", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 9, NameNp = "वडा ९" },
        new WardViewModel { Code = "W04030110", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 10, NameNp = "वडा १०" },
        new WardViewModel { Code = "W04030111", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 11, NameNp = "वडा ११" },
        new WardViewModel { Code = "W04030112", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 12, NameNp = "वडा १२" },
        new WardViewModel { Code = "W04030113", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 13, NameNp = "वडा १३" },
        new WardViewModel { Code = "W04030114", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 14, NameNp = "वडा १४" },
        new WardViewModel { Code = "W04030115", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 15, NameNp = "वडा १५" },
        new WardViewModel { Code = "W04030116", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 16, NameNp = "वडा १६" },
        new WardViewModel { Code = "W04030117", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 17, NameNp = "वडा १७" },
        new WardViewModel { Code = "W04030118", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 18, NameNp = "वडा १८" },
        new WardViewModel { Code = "W04030119", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 19, NameNp = "वडा १९" },
        new WardViewModel { Code = "W04030120", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 20, NameNp = "वडा २०" },
        new WardViewModel { Code = "W04030121", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 21, NameNp = "वडा २१" },
        new WardViewModel { Code = "W04030122", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 22, NameNp = "वडा २२" },
        new WardViewModel { Code = "W04030123", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 23, NameNp = "वडा २३" },
        new WardViewModel { Code = "W04030124", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 24, NameNp = "वडा २४" },
        new WardViewModel { Code = "W04030125", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 25, NameNp = "वडा २५" },
        new WardViewModel { Code = "W04030126", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 26, NameNp = "वडा २६" },
        new WardViewModel { Code = "W04030127", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 27, NameNp = "वडा २७" },
        new WardViewModel { Code = "W04030128", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 28, NameNp = "वडा २८" },
        new WardViewModel { Code = "W04030129", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 29, NameNp = "वडा २९" },
        new WardViewModel { Code = "W04030130", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 30, NameNp = "वडा ३०" },
        new WardViewModel { Code = "W04030131", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 31, NameNp = "वडा ३१" },
        new WardViewModel { Code = "W04030132", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 32, NameNp = "वडा ३२" },
        new WardViewModel { Code = "W04030133", MunicipalityCode = "M040301", MunicipalityName = "Pokhara Metropolitan City", WardNumber = 33, NameNp = "वडा ३३" }
    };

    // Helper method to generate wards for a municipality
    private static void AddWardsForMunicipality(List<WardViewModel> wardsList, string municipalityCode, string municipalityName, int wardCount)
    {
        for (int i = 1; i <= wardCount; i++)
        {
            string wardCode = municipalityCode.Replace("M", "W") + i.ToString("D2");
            string wardNumberNp = ConvertToNepaliNumber(i);
            wardsList.Add(new WardViewModel
            {
                Code = wardCode,
                MunicipalityCode = municipalityCode,
                MunicipalityName = municipalityName,
                WardNumber = i,
                NameNp = $"वडा {wardNumberNp}"
            });
        }
    }

    private static string ConvertToNepaliNumber(int number)
    {
        var nepaliDigits = new[] { "०", "१", "२", "३", "४", "५", "६", "७", "८", "९" };
        return string.Join("", number.ToString().Select(d => nepaliDigits[int.Parse(d.ToString())]));
    }

    // Initialize all wards for all municipalities
    private static List<WardViewModel> InitializeAllWards()
    {
        var allWards = new List<WardViewModel>(Wards);

        // Ward counts for municipalities (typical ranges: 5-35 wards per municipality)
        // This is a simplified approach - in reality, each municipality has a specific ward count
        // For now, we'll use a standard approach: 9 wards for rural municipalities, 11-15 for municipalities, 20-35 for sub-metro/metro
        
        foreach (var municipality in Municipalities)
        {
            int wardCount = 9; // Default for rural municipalities
            
            if (municipality.Name.Contains("Metropolitan City"))
                wardCount = municipality.Name.Contains("Kathmandu") ? 32 : 
                           municipality.Name.Contains("Lalitpur") ? 29 :
                           municipality.Name.Contains("Pokhara") ? 33 :
                           municipality.Name.Contains("Bharatpur") ? 29 :
                           municipality.Name.Contains("Biratnagar") ? 19 : 25;
            else if (municipality.Name.Contains("Sub-Metropolitan City"))
                wardCount = 19;
            else if (municipality.Name.Contains("Municipality") && !municipality.Name.Contains("Rural"))
                wardCount = 11;
            else if (municipality.Name.Contains("Rural Municipality"))
                wardCount = 9;

            // Only add if not already in the list
            if (!allWards.Any(w => w.MunicipalityCode == municipality.Code))
            {
                AddWardsForMunicipality(allWards, municipality.Code, municipality.Name, wardCount);
            }
        }

        return allWards;
    }

    private static readonly List<WardViewModel> AllWards = InitializeAllWards();

    public static List<ProvinceViewModel> GetProvinces()
    {
        return Provinces.ToList();
    }

    public static List<DistrictViewModel> GetDistricts(string provinceName)
    {
        return Districts
            .Where(d => d.ProvinceName.Equals(provinceName, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public static List<MunicipalityViewModel> GetMunicipalities(string districtName)
    {
        return Municipalities
            .Where(m => m.DistrictName.Equals(districtName, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public static List<WardViewModel> GetWards(string municipalityName)
    {
        return AllWards
            .Where(w => w.MunicipalityName.Equals(municipalityName, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}

