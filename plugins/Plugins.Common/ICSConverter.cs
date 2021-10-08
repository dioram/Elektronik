using UnityEngine;

namespace Elektronik.Plugins.Common
{
    /// <summary> Translates data between coordinate systems. </summary>
    public interface ICSConverter
    {
        /// <summary> Converts position and rotation to unity coordinate system. </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        void Convert(ref Vector3 pos, ref Quaternion rot);
        
        /// <summary> Overload for converting only position. </summary>
        /// <param name="pos"></param>
        void Convert(ref Vector3 pos);
        
        /// <summary> Overload for converting only rotation. </summary>
        /// <param name="rot"></param>
        void Convert(ref Quaternion rot);
        
        /// <summary> Converts position and rotation to unity coordinate system. </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns> Converted position and rotation. </returns>
        (Vector3 pos, Quaternion rot) Converted(Vector3 pos, Quaternion rot);
        
        /// <summary> Overload for converting position into new variable. </summary>
        /// <param name="pos"></param>
        /// <returns> Converted position. </returns>
        Vector3 Converted(Vector3 pos);
        
        /// <summary> Overload for converting rotation into new variable. </summary>
        /// <param name="rot"></param>
        /// <returns> Converted rotation. </returns>
        Quaternion Converted(Quaternion rot);
        
        /// <summary> Converts position and rotation from unity coordinate system. </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        void ConvertBack(ref Vector3 pos, ref Quaternion rot);

        /// <summary> Overload for converting position from unity coordinate system. </summary>
        /// <param name="pos"></param>
        void ConvertBack(ref Vector3 pos);

        /// <summary> Overload for converting rotation from unity coordinate system. </summary>
        /// <param name="rot"></param>
        void ConvertBack(ref Quaternion rot);

        /// <summary> Converts position and rotation from unity coordinate system. </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        (Vector3 pos, Quaternion rot)  ConvertedBack(Vector3 pos, Quaternion rot);

        /// <summary> Overload for converting position from unity coordinate system. </summary>
        /// <param name="pos"></param>
        Vector3 ConvertedBack(Vector3 pos);

        /// <summary> Overload for converting rotation from unity coordinate system. </summary>
        /// <param name="rot"></param>
        Quaternion ConvertedBack(Quaternion rot);
    }
}
