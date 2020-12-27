namespace AnalyticsService.Models.DBModels
{
    public partial class PlaybacksSlots
    {
        public int PlaybackSlotId { get; set; }
        public int? PlaybackId { get; set; }
        public int Value { get; set; }
        public int Slot { get; set; }
        public virtual Playbacks Playbacks { get; set; }
    }
}
