using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDIPaint
{
    public enum Tool
    {
        /// <summary>
        /// Инструменты рисования
        /// </summary>

        /// <summary>
        /// Перо
        /// </summary>
        Pen,

        /// <summary>
        /// Линия
        /// </summary>
        Line,

        /// <summary>
        /// Окружность
        /// </summary>
        Circle,

        /// <summary>
        /// Прямоугольник
        /// </summary>
        Rectangle,
        /// <summary>
        /// Ластик
        /// </summary>
        Lastik,


        /// <summary>
        /// Заливка
        /// </summary>
        Bucket,
        /// <summary>
        /// Многоульник n
        /// </summary>
        Polygon,
        /// <summary>
        /// Текст
        /// </summary>
        Text
    }
}
