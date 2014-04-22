using System;
using Leap;
using MotionLogger;

class SampleListener : Listener
{
    // CSV出力クラス
    private CSVExporter exporter = new CSVExporter();

    private Object thisLock = new Object();

    private void SafeWriteLine(String line)
    {
        lock (thisLock)
        {
            Console.WriteLine(line);
        }
    }

    public override void OnFrame(Controller controller)
    {
        // Get the most recent frame and report some basic information
        Frame frame = controller.Frame();

        this.exporter.Export(frame);

        SafeWriteLine("Frame id: " + frame.Id
                    + ", timestamp: " + frame.Timestamp
                    + ", hands: " + frame.Hands.Count
                    + ", fingers: " + frame.Fingers.Count
                    + ", tools: " + frame.Tools.Count
                    + ", gestures: " + frame.Gestures().Count);

        if (!frame.Hands.IsEmpty)
        {
            // Get the first hand
            Hand hand = frame.Hands[0];

            // Check if the hand has any fingers
            FingerList fingers = hand.Fingers;
            if (!fingers.IsEmpty)
            {
                // Calculate the hand's average finger tip position
                Vector avgPos = Vector.Zero;
                foreach (Finger finger in fingers)
                {
                    avgPos += finger.TipPosition;
                }
                avgPos /= fingers.Count;
                SafeWriteLine("Hand has " + fingers.Count
                            + " fingers, average finger tip position: " + avgPos);
            }

            // Get the hand's sphere radius and palm position
            SafeWriteLine("Hand sphere radius: " + hand.SphereRadius.ToString("n2")
                        + " mm, palm position: " + hand.PalmPosition);

            // Get the hand's normal vector and direction
            Vector normal = hand.PalmNormal;
            Vector direction = hand.Direction;

            // Calculate the hand's pitch, roll, and yaw angles
            SafeWriteLine("Hand pitch: " + direction.Pitch * 180.0f / (float)Math.PI + " degrees, "
                        + "roll: " + normal.Roll * 180.0f / (float)Math.PI + " degrees, "
                        + "yaw: " + direction.Yaw * 180.0f / (float)Math.PI + " degrees");
        }

        // Get gestures
        GestureList gestures = frame.Gestures();
        for (int i = 0; i < gestures.Count; i++)
        {
            Gesture gesture = gestures[i];

            switch (gesture.Type)
            {
                case Gesture.GestureType.TYPECIRCLE:
                    CircleGesture circle = new CircleGesture(gesture);

                    // Calculate clock direction using the angle between circle normal and pointable
                    String clockwiseness;
                    if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Math.PI / 4)
                    {
                        //Clockwise if angle is less than 90 degrees
                        clockwiseness = "clockwise";
                    }
                    else
                    {
                        clockwiseness = "counterclockwise";
                    }

                    float sweptAngle = 0;

                    // Calculate angle swept since last frame
                    if (circle.State != Gesture.GestureState.STATESTART)
                    {
                        CircleGesture previousUpdate = new CircleGesture(controller.Frame(1).Gesture(circle.Id));
                        sweptAngle = (circle.Progress - previousUpdate.Progress) * 360;
                    }

                    SafeWriteLine("Circle id: " + circle.Id
                                   + ", " + circle.State
                                   + ", progress: " + circle.Progress
                                   + ", radius: " + circle.Radius
                                   + ", angle: " + sweptAngle
                                   + ", " + clockwiseness);
                    break;
                case Gesture.GestureType.TYPESWIPE:
                    SwipeGesture swipe = new SwipeGesture(gesture);
                    SafeWriteLine("Swipe id: " + swipe.Id
                                   + ", " + swipe.State
                                   + ", position: " + swipe.Position
                                   + ", direction: " + swipe.Direction
                                   + ", speed: " + swipe.Speed);
                    break;
                case Gesture.GestureType.TYPEKEYTAP:
                    KeyTapGesture keytap = new KeyTapGesture(gesture);
                    SafeWriteLine("Tap id: " + keytap.Id
                                   + ", " + keytap.State
                                   + ", position: " + keytap.Position
                                   + ", direction: " + keytap.Direction);
                    break;
                case Gesture.GestureType.TYPESCREENTAP:
                    ScreenTapGesture screentap = new ScreenTapGesture(gesture);
                    SafeWriteLine("Tap id: " + screentap.Id
                                   + ", " + screentap.State
                                   + ", position: " + screentap.Position
                                   + ", direction: " + screentap.Direction);
                    break;
                default:
                    SafeWriteLine("Unknown gesture type.");
                    break;
            }
        }

        if (!frame.Hands.IsEmpty || !frame.Gestures().IsEmpty)
        {
            SafeWriteLine("");
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
