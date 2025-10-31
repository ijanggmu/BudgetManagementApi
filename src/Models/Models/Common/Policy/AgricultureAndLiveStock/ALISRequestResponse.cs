using System;
using System.Collections.Generic;

namespace Models.Common.Policy.AgricultureAndLiveStock
{
    public class MasterPolicyCode
    {
        public string name { get; set; }
        public string name_nep { get; set; }
        public string code { get; set; }
    }

    public class AcceptedPolicies
    {
        public string policyCode { get; set; }
        public List<AcceptedPolicy> data { get; set; }
    }

    public class DraftPolicies
    {
        public bool? success { get; set; }
        public string message { get; set; }
        public DraftPolicy data { get; set; }
    }

    public class AcceptedPolicy
    {
        public string agent_license_no { get; set; }
        
        public string agent_name { get; set; }
        
        public string technician_license_no { get; set; }
        
        public string technician_code { get; set; }
        
        public string technician_name { get; set; }
        
        public string application_date { get; set; }
        
        public string applicant { get; set; }
        
        public string policy_name { get; set; }
        
        public string draft_policy_no { get; set; }
    }


    public class DraftPolicy
    {
        public int beema_period_year { get; set; }
        public int beema_period_month { get; set; }
        public int beema_period_day { get; set; }
        public string organization { get; set; }
        public string sub_sector { get; set; }
        public int sub_sector_code { get; set; }
        public string organization_code { get; set; }
        public Applicant applicant { get; set; }
        public Premium premium { get; set; }
        public FirmInfo firm_info { get; set; }
        public LoanBankInfo loan_bank_info { get; set; }
        public NomineeInfo nominee_info { get; set; }
        public List<InsuredProperties> insured_property { get; set; }
        public OtherDetail other_detail { get; set; }
    }

    public class LoanBankInfo
    {
        public string name { get; set; }
        public string branch { get; set; }
        public string loan_amount { get; set; }
        public Address address { get; set; }
    }

    public class Address
    {
        public string province { get; set; }
        public int province_code { get; set; }
        public string district { get; set; }
        public int district_code { get; set; }
        public string municipality { get; set; }
        public int municipality_code { get; set; }
        public string ward { get; set; }
        public string kitta_no { get; set; }
        public string sanket_no { get; set; }
    }

    public class Premium
    {
        public decimal beemanka_amount_before_beemanka_percent { get; set; }
        public string beemanka_percent { get; set; }
        public string total_beema_amount { get; set; }
        public string direct_discount_percent { get; set; }
        public string direct_discount_amount { get; set; }
        public string no_claim_discount_percent { get; set; }
        public string no_claim_discount_amount { get; set; }
        public string subsidy_percent { get; set; }
        public string subsidized_premium { get; set; }
        public string accident_insurance_amount { get; set; }
        public string premium_amount { get; set; }
        public string ticket_amount { get; set; }
        public string accident_insurance_count { get; set; }
    }


    public class Applicant
    {
        public string name { get; set; }
        public string name_nep { get; set; }
        public string customerId { get; set; }
        public string citizenship_no { get; set; }
        public string citizenship_issued_date { get; set; }
        public string citizenship_issued_district { get; set; }
        public int? citizenship_issued_district_code { get; set; }
        public int? document_type_id { get; set; }
        public string company_reg_no { get; set; }
        public string company_reg_date { get; set; }
        public string company_registration_type { get; set; }
        public string document_type { get; set; }
        public string document_no { get; set; }
        public string document_issued_date { get; set; }
        public string document_expiry_date { get; set; }
        public string email { get; set; }
        public string mobile_no { get; set; }
        public string type { get; set; }
        public Address address { get; set; }
        public string occupation { get; set; }
        public int? occupation_id { get; set; }
    }


    public class FirmInfo
    {
        public string firm_name { get; set; }
        public string firm_estd_date { get; set; }
        public string firm_darta_no { get; set; }
        public Address firm_address { get; set; }
    }

    public class NomineeInfo
    {
        public string name { get; set; }
        public string citizenship_no { get; set; }
        public string mobile_no { get; set; }
        public string email { get; set; }
        public string relation { get; set; }
        public string father_name { get; set; }
        public string mother_name { get; set; }
    }

    public class InsuredProperties
    {
        //Common
        
        public int sub_sector_code { get; set; } 
        public string breed { get; set; }
        public int breed_id { get; set; }  
        public string purchased_date { get; set; }
        public string sub_sector { get; set; }
        public string purchased_amount { get; set; }
        
        //Livestock
        
        public string sanket_patta_np { get; set; }
        public string classification { get; set; }
        public int classification_id { get; set; }
        public string breed_type { get; set; }
        public int breed_type_id { get; set; }
        public int total_age_days { get; set; }
        public string color { get; set; }
        public string weight { get; set; }
        public string raised_type { get; set; }
        public int raised_type_id { get; set; }
        public string is_purchased { get; set; }
        public string beemanka_amount { get; set; }
        
        
        
        //grain
        public string grain_name { get; set; }
        public string plant_breed { get; set; }
    
        public string plant_per_ha { get; set; }
        public string farming_type { get; set; }
        public string total_area { get; set; }
        public string area_hectare { get; set; }
        public string production_per_hectare { get; set; }
        public string avg_district_producion_t_per_ha { get; set; }
        public string previous_production_t_per_ha { get; set; }
        public string price_per_t { get; set; }
        public string cost_per_ton { get; set; }

        //fish
        
        public string farming_technique { get; set; }

        public string types_of_fish { get; set; }
        public string stocking_rate { get; set; }

        public string average_weight { get; set; }

        public string water_reservoir_area { get; set; }
        
        public string total_fish_count { get; set; }
    }

    public class Purpose
    {
        public string name { get; set; }
        public int id { get; set; }
        
    }
    public class FarmerDetails
    {
        //fish
        public string pond_area { get; set; }
        public string water_reservior_area { get; set; }
        public string pond_depth { get; set; }
        public string pond_owner { get; set; }
        public string fish_cost_price { get; set; }
        public string purchased_source { get; set; }
        public string bhura_type { get; set; }
        public string machapalan_gareko_start_date { get; set; }
        public string harvest_permission_date { get; set; }
        public bool is_flood_affected_area { get; set; }
        public string fish_farm_service { get; set; }
        public bool has_taken_training { get; set; }
        public string training_provider_org { get; set; }
        public bool has_taken_facility { get; set; }
        public string fish_farm_facilities { get; set; }
        public bool has_pervious_illness { get; set; }
        public string loss_due_to_illness { get; set; }
        public string training_period { get; set; }
        public bool is_ill { get; set; }
        public List<Purpose> purpose { get; set; }

        
        
        public string group_individual { get; set; }
        public string group_name { get; set; }
        public string group_email { get; set; }
        public string group_mobile_no { get; set; }
        public string group_phone_no { get; set; }
        public string group_address { get; set; }
        
        public string plantation_date { get; set; }
        public string previous_crop_detail { get; set; }
        public bool? has_inter_crop { get; set; }
        public string inter_crop_name { get; set; }
        public bool? irrigation_availablity { get; set; }
        public string compost_kg { get; set; }
        public string d_a_p_gram { get; set; }
        public string urea_gram { get; set; }
        public string potash_gram { get; set; }
        public string other { get; set; }
        public string regular_training_detail { get; set; }
        public bool? hormone_applied { get; set; }
        public string hormone_details { get; set; }
        public string technical_advice { get; set; }
        public bool? plantation_training { get; set; }
        public string training_provider_org_name { get; set; }
        public string training_duration { get; set; }
        public string training_start_date { get; set; }
        public bool? has_agricultural_facilities { get; set; }
        public bool? had_previous_illness { get; set; }
        public string previous_loss_amount { get; set; }
        public bool? has_shade_plant { get; set; }
        public bool? shade_plant_type { get; set; }
        public string shade_plant_count { get; set; }
        public bool? shade_plant_age { get; set; }
        public List<PreviousBeema> previous_beema { get; set; }
    }

    public class PreviousBeema
    {
        public string organization { get; set; }
        public decimal claimed_count { get; set; }
        public string claimed_amount { get; set; }
    }

    public class Technician
    {
        //fish
        public string livestock_area_type { get; set; }
        public string types_of_fish { get; set; }
        public string pond_health_status { get; set; }
        public string fish_status { get; set; }
        public bool has_fulfilled_standards { get; set; }
        public bool has_used_medicine { get; set; }
        public bool is_fish_ill { get; set; }
        public bool has_balanced_food { get; set; }
        public bool has_safety_measures_adopted { get; set; }
        public bool has_illness_symptom { get; set; }
        public string medicine_details { get; set; }
        public string stocking_rate { get; set; }
        public string pond_construction_cost { get; set; }
        public string illness_detail { get; set; }
        public string other_fish_mgmt_details { get; set; }
        public string other_animal_details { get; set; }
        public string illness_symptom_details { get; set; }
        public string reject_reason { get; set; }
        public string suggestion { get; set; }
        
        

        
        
        public string sub_sector { get; set; }
        public string recommended_area { get; set; }
        public string requested_quantity { get; set; }
        public string plant_area_type { get; set; }
        public string plantation_altitude { get; set; }
        public string land_modaha { get; set; }
        public string irrigation_type { get; set; }
        public bool? replaced_dead_plant { get; set; }
        public bool? treated_after_pruning { get; set; }
        public bool? hormone_applied { get; set; }
        public string hormone_details { get; set; }
        public bool? has_inter_crop { get; set; }
        public string inter_crop_name { get; set; }
        public string fertilizer_per_unit_area { get; set; }
        public bool? has_pest_infestation { get; set; }
        public string pest_details { get; set; }
        public bool? had_previous_illness { get; set; }
        public string previous_illness_detail { get; set; }
        public bool? has_reject_reason { get; set; }
        public string reject_reason_detail { get; set; }
        public string risk_acceptance_suggestion { get; set; }
        public string technical_test_detail { get; set; }
        public string technical_test_decision { get; set; }
        public string air_water_condition { get; set; }
        public bool? need_more_technical_test { get; set; }
    }

    public class OtherDetail
    {
        public FarmerDetails farmer { get; set; }
        public Technician technician { get; set; }
        
        public string agent_license_no { get; set; }
        
        public string agent_name { get; set; }
        
        public string technician_license_no { get; set; }
        
        public string technician_code { get; set; }
        
        public string technician_name { get; set; }
    }   
}
