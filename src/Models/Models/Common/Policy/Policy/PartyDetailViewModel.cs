using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Common.Policy.Policy
{
    public class PartyDetailViewModel
    {
        public string PartyCode { get; set; }
        public string PartyTypeCode { get; set; }
        public string Status { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string FirstNameNp { get; set; }
        public string MiddleNameNp { get; set; }
        public string LastNameNp { get; set; }
        public string FieldOfficer { get; set; }
        public string ClassId { get; set; }
        public string Staff { get; set; }
        public string Email { get; set; }
        public string MultipleFinancer { get; set; }

        #region Contact Information
        public string Phone { get; set; }
        public string MaritialStatus { get; set; }
        public string SecondaryEmail { get; set; }
        public string Pa_Province { get; set; }
        public string Pa_District { get; set; }
        public string Pa_City { get; set; }
        public string Pa_Municipality { get; set; }
        public int Pa_Ward { get; set; }
        public string Pa_StreetAddress { get; set; }
        public string Pa_HouseNumber { get; set; }
        public string Tm_Province { get; set; }
        public string Tm_District { get; set; }
        public string Tm_City { get; set; }
        public string Tm_Municipality { get; set; }
        public int Tm_Ward { get; set; }
        public string Tm_StreetAddress { get; set; }
        public string Tm_HouseNumber { get; set; }
        public string Mobile { get; set; }
        public string Country { get; set; }
        public string Secondary_Phone { get; set; }
        public string Secondary_Email { get; set; }
        public string ProfilePicture { get; set; }
        public string NotesByStaff { get; set; }
        public string ContactId { get; set; }
        #endregion contact

        #region Contactkyc Information
        public DateTime? Dob_AD { get; set; }
        public DateTime? Dob_BS { get; set; }
        public string Citizenship_No { get; set; }
        public string Citizenship_Issue_District { get; set; }
        public DateTime? Citizenship_Issue_date { get; set; }
        public string Passport_Number { get; set; }
        public DateTime? Passport_Issue_Date { get; set; }
        public string Passport_Issue_Place { get; set; }
        public DateTime? Passport_Expiry_Date { get; set; }
        public string License_Number { get; set; }
        public string License_Place { get; set; }
        public DateTime? License_Expiry_Date { get; set; }
        public string PanVat_Number { get; set; }
        public string VoterId_Number { get; set; }
        //Json Object
        public string Occupation { get; set; }
        //Json Object
        public string Nature_Of_Business { get; set; }
        public bool Own_Business { get; set; }
        //Json Object
        public string Work_Experience_details { get; set; }
        public string Total_Annual_Income_NPR { get; set; }
        //Json Object
        public string Education { get; set; }
        //Json Object
        public string Religion { get; set; }
        public string Spouse_FullName { get; set; }
        public string Father_FullName { get; set; }
        public string Mother_FullName { get; set; }
        public string GrandFather_FullName { get; set; }
        public string GrandMother_FullName { get; set; }
        //Json Object
        public string Son { get; set; }
        //Json Object
        public string Daughter { get; set; }
        public string Daughter_InLaw { get; set; }
        public string Father_InLaw { get; set; }
        public string Mother_InLaw { get; set; }
        public string Client_Classification { get; set; }
        #endregion contatckyc

        #region Contact Document Information
        public string Citizenship_File { get; set; }
        public string Passport_File { get; set; }
        public string License_File { get; set; }
        public string Pancard_File { get; set; }
        public string Votercard_File { get; set; }
        #endregion
    }
}
