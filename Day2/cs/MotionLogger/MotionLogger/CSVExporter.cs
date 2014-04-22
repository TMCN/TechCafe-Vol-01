using System.IO;
using System.Text;
using Leap;

namespace MotionLogger
{
    public class CSVExporter
    {
        static private readonly string FilePath = "log.csv";
        static private readonly string Delimiter = ",";

        // コンストラクタ、ヘッダ出力
        public CSVExporter()
        {
            StreamWriter writer = new StreamWriter(FilePath);
            writer.WriteLine("id,timestamp,hands,fingers,tools,gestures,position_x,position_y,position_z");
            writer.Close();
        }

        // CSVレコード出力
        public void Export(Frame frame)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(frame.Id).Append(Delimiter);
            sb.Append(frame.Timestamp).Append(Delimiter);
            sb.Append(frame.Hands.Count).Append(Delimiter);
            sb.Append(frame.Fingers.Count).Append(Delimiter);
            sb.Append(frame.Tools.Count).Append(Delimiter);
            sb.Append(frame.Gestures().Count).Append(Delimiter);
            if (!frame.Hands.IsEmpty)
            {
                Hand hand = frame.Hands[0];
                FingerList fingers = hand.Fingers;
                if (!fingers.IsEmpty)
                {
                    Vector avgPos = Vector.Zero;
                    foreach (Finger finger in fingers)
                    {
                        avgPos += finger.TipPosition;
                    }
                    avgPos /= fingers.Count;
                    sb.Append(avgPos.x).Append(Delimiter).Append(avgPos.y).Append(Delimiter).Append(avgPos.z);
                }
            }
            else
            {
                sb.Append("0,0,0");
            }

            StreamWriter writer = new StreamWriter(FilePath, true);
            writer.WriteLine(sb.ToString());
            writer.Close();
        }
    }
}
