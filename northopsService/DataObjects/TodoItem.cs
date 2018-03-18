using Microsoft.Azure.Mobile.Server;

namespace northopsService.DataObjects
{
    public partial class TodoItem : EntityData
    {
        public string Text { get; set; }

        public bool Complete { get; set; }
    }
}