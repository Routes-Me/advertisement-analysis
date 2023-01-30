namespace AdvertisementAnalysisService.Models.DBModels
{
    public partial class PlaybackSlots
    {
        public int PlaybackSlotId { get; set; }
        public int? PlaybackId { get; set; }
        public int Value { get; set; }
        public PlaybackSlotsEnum Slot { get; set; }
        public virtual Playback Playbacks { get; set; }
    }
    public enum PlaybackSlotsEnum
    {
        morning,
        noon,
        evening,
        night
    }
}
