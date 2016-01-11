using System.Windows.Forms;

namespace Konamiman.NestorMSX.Host
{
    public class DoubleBufferedPictureBox : PictureBox
    {
        public DoubleBufferedPictureBox() : base()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }
    }
}
