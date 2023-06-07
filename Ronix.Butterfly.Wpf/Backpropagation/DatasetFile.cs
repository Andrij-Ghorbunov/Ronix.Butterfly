using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ronix.Butterfly.Wpf.Backpropagation
{
    public class DatasetFile
    {
        public List<DatasetItem> Winning { get; }
        public List<DatasetItem> Losing { get; }
        public List<DatasetItem> Draws { get; }

        public DatasetFile()
        {
            Winning = new();
            Losing = new();
            Draws = new();
        }

        public void Save(string path)
        {
            using var file = new FileStream(path, FileMode.Create);
            Save(file);
        }

        public void Save(Stream stream)
        {
            using var writer = new BinaryWriter(stream);
            writer.Write((byte)68);
            writer.Write((byte)83); // DS - magic letters for a DataSet file
            writer.Write(Winning.Count);
            writer.Write(Losing.Count);
            writer.Write(Draws.Count);
            foreach (var item in Winning)
            {
                WriteItem(writer, item);
            }
            foreach (var item in Losing)
            {
                WriteItem(writer, item);
            }
            foreach (var item in Draws)
            {
                WriteItem(writer, item);
            }
        }
        public static DatasetFile Load(string path)
        {
            using var file = new FileStream(path, FileMode.Open);
            return Load(file);
        }

        public static DatasetFile Load(Stream stream)
        {
            var r = new DatasetFile();
            using var reader = new BinaryReader(stream);
            var byte0 = reader.ReadByte();
            var byte1 = reader.ReadByte();
            if (byte0 != 68 || byte1 != 83)
                throw new Exception("This is not a dataset file!");
            var winningCount = reader.ReadInt32();
            var losingCount = reader.ReadInt32();
            var drawCount = reader.ReadInt32();
            for (var i = 0; i < winningCount; i++)
            {
                r.Winning.Add(ReadItem(reader));
            }
            for (var i = 0; i < losingCount; i++)
            {
                r.Losing.Add(ReadItem(reader));
            }
            for (var i = 0; i < drawCount; i++)
            {
                r.Draws.Add(ReadItem(reader));
            }
            return r;
        }

        private void WriteItem(BinaryWriter writer, DatasetItem item)
        {
            for (var i = 0; i < 9; i++)
            {
                var b = (byte)(item.InputData[4 * i] + 4 * item.InputData[4 * i + 1] + 16 * item.InputData[4 * i + 2] + 64 * item.InputData[4 * i + 3]);
                writer.Write(b);
            }
            writer.Write((byte)item.ScoreSum);
            writer.Write((sbyte)item.ScoreDifference);
            writer.Write((byte)item.MoveSelected);
        }

        private static DatasetItem ReadItem(BinaryReader reader)
        {
            var data = new byte[36];
            var bytes = reader.ReadBytes(9);
            for (var i = 0; i < 9; i++)
            {
                var b = bytes[i];
                data[4 * i] = (byte)(b % 4);
                b /= 4;
                data[4 * i + 1] = (byte)(b % 4);
                b /= 4;
                data[4 * i + 2] = (byte)(b % 4);
                b /= 4;
                data[4 * i + 3] = b;
            }
            var scoreSum = reader.ReadByte();
            var scoreDiff = reader.ReadSByte();
            var moveSelected = reader.ReadByte();
            return new DatasetItem(data, scoreSum, scoreDiff, moveSelected);
        }
    }
}
