using Newtonsoft.Json;

namespace TaskManagerTut.DTO

{
    public class TaskModel
    {
        [JsonProperty(PropertyName = "uId", NullValueHandling = NullValueHandling.Ignore)]
        public string UId { get; set; }

        [JsonProperty(PropertyName = "taskId", NullValueHandling = NullValueHandling.Ignore)]
        public string TaskNo { get; set; }

        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
    }
}
