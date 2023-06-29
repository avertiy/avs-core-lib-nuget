#nullable enable
namespace AVS.CoreLib.Trading.TA
{
    public class TAInputs
    {
        public int AvgVolumeLength { get; set; } = 12;
        public int MA14Length { get; set; } = 14;
        public int MA21Length { get; set; } = 21;
        public int MA50Length { get; set; } = 50;
        public int MA100Length { get; set; } = 100;
        public int BBLength { get; set; } = 21;
        public decimal StdDevMul { get; set; } = 2.5m;
        public decimal StdDevMulNarrow { get; set; } = 1;
    }
}