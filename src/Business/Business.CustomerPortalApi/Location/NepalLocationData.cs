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

        // Rupandehi District
        new MunicipalityViewModel { Code = "D051201", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Butwal Sub-Metropolitan City", NameNp = "बुटवल उपमहानगरपालिका" },
        new MunicipalityViewModel { Code = "D051202", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Siddharthanagar Municipality", NameNp = "सिद्धार्थनगर नगरपालिका" },
        new MunicipalityViewModel { Code = "D051203", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Tilottama Municipality", NameNp = "तिलोत्तमा नगरपालिका" },
        new MunicipalityViewModel { Code = "D051204", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Lumbini Sanskritik Municipality", NameNp = "लुम्बिनी सांस्कृतिक नगरपालिका" },
        new MunicipalityViewModel { Code = "D051205", DistrictCode = "D0512", DistrictName = "Rupandehi", Name = "Sainamaina Municipality", NameNp = "सैनामैना नगरपालिका" },

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
        new MunicipalityViewModel { Code = "M010411", DistrictCode = "D0104", DistrictName = "Jhapa", Name = "Shivasatakshi Municipality", NameNp = "शिवसताक्षी नगरपालिका" }
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
        return Wards
            .Where(w => w.MunicipalityName.Equals(municipalityName, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}

