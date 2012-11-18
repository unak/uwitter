using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Uwitter
{
    [DataContract]
    class Timeline
    {
        //[DataMember]
        //public Coordinates coordinates { get; set; }
        [DataMember]
        public bool truncated { get; set; }
        [DataMember]
        public string created_at { get; set; }
        [DataMember]
        public bool favaorited { get; set; }
        [DataMember]
        public string id_str { get; set; }
        [DataMember]
        public string in_reply_to_user_id_str { get; set; }
        [DataMember]
        public Entities entities { get; set; }
        [DataMember]
        public string text { get; set; }
        //[DataMember]
        //public string[] contributors { get; set; }
        [DataMember]
        public decimal id { get; set; }
        [DataMember]
        public int retweet_count { get; set; }
        [DataMember]
        public string in_reply_to_status_id_str { get; set; }
        //[DataMember]
        //public string geo { get; set; }
        [DataMember]
        public bool retweeted { get; set; }
        [DataMember]
        public Timeline retweeted_status { get; set; }
        [DataMember]
        public decimal? in_reply_to_user_id { get; set; }
        //[DataMember]
        //public string place { get; set; }
        [DataMember]
        public string source { get; set; }
        [DataMember]
        public TwitterUser user { get; set; }
        [DataMember]
        public string in_reply_to_screen_name { get; set; }
        [DataMember]
        public decimal? in_reply_to_status_id { get; set; }
    }

    [DataContract]
    class Coordinates
    {
        [DataMember]
        public double[] coordinates { get; set; }
        [DataMember]
        public string type { get; set; }
    }

    [DataContract]
    class Entities
    {
        [DataMember]
        public TwitterUrl[] urls { get; set; }
        //[DataMember]
        //public string[] hashtags { get; set; }
        [DataMember]
        public Mention[] user_mentions { get; set; }
    }

    [DataContract]
    class TwitterUrl
    {
        [DataMember]
        public string expanded_url { get; set; }
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public Int64[] indices { get; set; }
        [DataMember]
        public string display_url { get; set; }
    }

    [DataContract]
    class Mention
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string id_str { get; set; }
        [DataMember]
        public decimal id { get; set; }
        [DataMember]
        public Int64[] indices { get; set; }
        [DataMember]
        public string screen_name { get; set; }
    }

    [DataContract]
    class TwitterUser
    {
        [DataMember]
        public string name { get; set; }
        //[DataMember]
        //public string profile_sidebar_fill_color { get; set; }
        //[DataMember]
        //public bool profile_background_tile { get; set; }
        //[DataMember]
        //public string profile_sidebar_border_color { get; set; }
        [DataMember]
        public string profile_image_url { get; set; }
        //[DataMember]
        //public string created_at { get; set; }
        //[DataMember]
        //public string location { get; set; }
        //[DataMember]
        //public string follow_request_sent { get; set; }
        [DataMember]
        public string id_str { get; set; }
        //[DataMember]
        //public bool is_translator { get; set; }
        //[DataMember]
        //public UserEntities entities { get; set; }
        //[DataMember]
        //public bool default_profile { get; set; }
        //[DataMember]
        //public string url { get; set; }
        //[DataMember]
        //public bool contributors_enabled { get; set; }
        //[DataMember]
        //public int favourites_count { get; set; }
        //[DataMember]
        //public int utc_offset { get; set; }
        //[DataMember]
        //public string profile_image_url_https { get; set; }
        [DataMember]
        public decimal id { get; set; }
        //[DataMember]
        //public int listed_count { get; set; }
        //[DataMember]
        //public bool profile_use_background_image { get; set; }
        //[DataMember]
        //public string profile_text_color { get; set; }
        //[DataMember]
        //public int followers_count { get; set; }
        //[DataMember]
        //public string lang { get; set; }
        [DataMember]
        public bool @protected { get; set; }
        //[DataMember]
        //public bool geo_enabled { get; set; }
        //[DataMember]
        //public bool notifications { get; set; }
        //[DataMember]
        //public string description { get; set; }
        //[DataMember]
        //public string profile_background_color { get; set; }
        [DataMember]
        public bool verified { get; set; }
        //[DataMember]
        //public string time_zone { get; set; }
        //[DataMember]
        //public string profile_background_image_url_https { get; set; }
        //[DataMember]
        //public int statuses_count { get; set; }
        //[DataMember]
        //public string profile_background_image_url { get; set; }
        //[DataMember]
        //public bool default_profile_image { get; set; }
        //[DataMember]
        //public int friends_count { get; set; }
        [DataMember]
        public bool? following { get; set; }
        //[DataMember]
        //public bool show_all_inline_media { get; set; }
        [DataMember]
        public string screen_name { get; set; }
    }
}
