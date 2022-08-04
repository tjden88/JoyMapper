namespace JoyMapper.Models
{
    /// <summary>
    /// Состояние кнопок и осей джойстика
    /// </summary>
    internal class JoyState
    {
        public struct AxisState
        {
            public int X;
            public int Y;
            public int Z;
            public int Rx;
            public int Ry;
            public int Rz;
            public int Slider1;
            public int Slider2;
        }


        /// <summary> Состояние кнопок джойстика </summary>
        public bool[] Buttons { get; set; }

        /// <summary> Значение указателя вида 1 </summary>
        public int Pow1Value { get; set; }

        /// <summary> Значение указателя вида 2 </summary>
        public int Pow2Value { get; set; }

        /// <summary> Состояние осей джойстика </summary>
        public AxisState AxisValues { get; set; }
    }
}
