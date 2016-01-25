namespace Konamiman.NestorMSX.Hardware
{
    /// <summary>
    /// Screen modes officially supported by V9938.
    /// Values are composed by using the values of M5 M4 M3 M2 M1 to compose a binary number,
    /// where M5 is the MSB and M1 is the LSB.
    /// </summary>
    public enum ScreenMode
    {
        Text1 = 1,       //SCREEN 0, 40 columns
        Text2 = 9,       //SCREEN 0, 80 columns
        Multicolor = 2,  //SCREEN 3
        Graphic1 = 0,    //SCREEN 1
        Graphic2 = 4,    //SCREEN 2
        Graphic3 = 8,    //SCREEN 4
        Graphic4 = 12,   //SCREEN 5
        Graphic5 = 16,   //SCREEN 6
        Graphic6 = 20,   //SCREEN 7
        Graphic7 = 28    //SCREEN 8
    }
}
