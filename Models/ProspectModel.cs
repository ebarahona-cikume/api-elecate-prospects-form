
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Google.Protobuf.WellKnownTypes;

namespace ApiElecateProspectsForm.Models
{
    public class ProspectModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int pro_id { get; set; }

        public string? pro_sper { get; set; }

        public string? pro_spr2 { get; set; }

        public string? pro_spr3 { get; set; }

        public string? pro_spr4 { get; set; }

        public string? pro_spr5 { get; set; }

        public string? pro_rep { get; set; }

        public DateTime? pro_date { get; set; }

        public string? pro_name { get; set; }

        public string? pro2name { get; set; }

        public string? pro_sal { get; set; }

        public string? pro_lnam { get; set; }

        public string? pro_mi { get; set; }

        public string? pro_fnam { get; set; }

        public string? pro_title { get; set; }

        public string? pro_nicknm { get; set; }

        public string? pro_refrby { get; set; }

        public string? pro_secy { get; set; }

        public string? pro_adres1 { get; set; }

        public string? pro_adres2 { get; set; }

        public string? pro_city { get; set; }

        public string? pro_st { get; set; }

        public string? pro_zip { get; set; }

        public string? pro_ctry { get; set; }

        public string? pro_ophn { get; set; }

        public decimal? ophn_ext { get; set; }

        public string? pro_hphn { get; set; }

        public string? pro_fax { get; set; }

        public string? pro_mphn { get; set; }

        public DateTime? pro_lpd { get; set; }

        public string? pro_prby { get; set; }

        public string? pro_terms { get; set; }

        public string? pro_vtype { get; set; }

        public bool? addr_chang { get; set; }

        public bool? new_pro { get; set; }

        public string? taxid { get; set; }

        public string? mailtype { get; set; }

        public string? e_mail { get; set; }

        public string? pro_notes { get; set; }

        public string? pro_refby { get; set; }

        public string? last_actn { get; set; }

        public string? nxt_actns { get; set; }

        public DateTime? call_back { get; set; }

        public string? website { get; set; }

        public string? pro_spouse { get; set; }

        public string? pro_spcare { get; set; }

        public string? lst_actn { get; set; }

        public decimal? an_budget { get; set; }

        public DateTime? year_end { get; set; }

        public string? key_cont { get; set; }

        public string? products { get; set; }

        public string? strategy { get; set; }

        public string? pro_freq { get; set; }

        public string? pro_size { get; set; }

        public string? pro_region { get; set; }

        public string? pro_status { get; set; }

        public DateTime? pro_specdt { get; set; }

        public DateTime? pro_apoint { get; set; }

        public string? pro_speccare { get; set; }

        public string? pro_dttype { get; set; }

        public bool pro_appdur { get; set; } = false;

        public string? pro_loc { get; set; }

        public string? pro_priort { get; set; }

        public string? pro_busin { get; set; }

        public string? pro_wedpro { get; set; }

        public DateTime? pro_todo { get; set; }

        public int? pro_cusid { get; set; }

        public string? pro_cuser { get; set; }

        public DateTime? pro_create { get; set; }

        public string? pro_euser { get; set; }

        public DateTime? pro_edit { get; set; }

        public decimal event_bdgt { get; set; } = 0;

        public string? cont_type { get; set; }

        [Timestamp]
        public byte[]? timestamp_column { get; set; }

        public string? proconotes { get; set; }

        public string? proCostcenter { get; set; }

        public bool? optout { get; set; }

        public DateTime? optoutdt { get; set; }

        public string taxid2 { get; set; } = string.Empty;

        public string pro_phase { get; set; } = string.Empty;

        public int? photoFk { get; set; }

        public int? hubspot_submitted_at { get; set; }

        public int? org_fk { get; set; }
    }
}