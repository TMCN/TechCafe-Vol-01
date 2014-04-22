using System;
using Leap;

class SampleListener : Listener
{
    private Object thisLock = new Object();

    private void SafeWriteLine(String line)
    {
        lock (thisLock)
        {
            Console.WriteLine(line);
        }
    }

    //前回チェック時間とその時のFrame
    private DateTime checkedTime = DateTime.MinValue;
    private Frame checkedFrame = null;

    public override void OnFrame(Controller controller)
    {
        // Get the most recent frame and report some basic information
        Frame frame = controller.Frame();

        if (!frame.Hands.IsEmpty)
        {
            if ((DateTime.Now - checkedTime) > new TimeSpan(0, 0, 0, 0, 500))
            {
                // 500ミリ秒ごとに前回位置との差分を求める
                // 移動量がしきい値を超えていたらイベントとして認識するする
                if (this.checkedFrame != null)
                {
                    Vector diff = frame.Hands[0].Translation(this.checkedFrame);
                    //SafeWriteLine("X移動量: " + diff.x);
                    if (diff.x > 150)
                    {
                        SafeWriteLine("右フリックしました！ :" + diff.x);
                    }
                    if (diff.x < -150)
                    {
                        SafeWriteLine("左フリックしました！ :" + diff.x);
                    }
                }
                this.checkedTime = DateTime.Now;
                this.checkedFrame = frame;
            }
        }
    }
}

class Sample
{
    public static void Main()
    {
        SampleListener listener = new SampleListener();
        Controller controller = new Controller();
        controller.AddListener(listener);
        Console.WriteLine("Press Enter to quit...");
        Console.ReadLine();
        controller.RemoveListener(listener);
        controller.Dispose();
    }
}
