using System.Collections.Generic;
using System.Text;

namespace AVS.CoreLib.Console.ColorFormatting.Tags
{
    public class ColorTagProcessor : TagProcessor
    {
        private readonly List<TagProcessor> _processors = new List<TagProcessor>();

        public ColorTagProcessor()
        {
            AddTagProcessor(new CTagProcessor());
            AddTagProcessor(new RgbTagProcessor());
        }

        public void AddTagProcessor(TagProcessor tagProcessor)
        {
            _processors.Add(tagProcessor);
        }

        public void Clear()
        {
            _processors.Clear();
        }

        public override void Process(StringBuilder sb)
        {
            foreach (var processor in _processors)
                processor.Process(sb);
        }
    }
}