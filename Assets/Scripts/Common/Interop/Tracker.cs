using System;

namespace Elektronik.Common.Interop
{
    using UnityMatrix4x4 = UnityEngine.Matrix4x4;
    using UnityPose = UnityEngine.Pose;
    using UnityQuaternion = UnityEngine.Quaternion;
    using UnityVector3 = UnityEngine.Vector3;

    public class Tracker
    {
        IntPtr m_trackerPtr;

        public Tracker()
        {
            m_trackerPtr = OMWSlamWrapper.createTracker();
        }

        public bool GetAbsPose(out UnityPose pose)
        {
            var nativePose = new Pose();
            bool result = OMWSlamWrapper.getAbsPose(m_trackerPtr, ref nativePose);
            pose = new UnityPose();
            if (result)
            {
                pose.position = new UnityVector3(nativePose.position.a[0], nativePose.position.a[1], nativePose.position.a[2]);
                pose.rotation = new UnityQuaternion(nativePose.rotation.a[1], nativePose.rotation.a[2], nativePose.rotation.a[3], nativePose.rotation.a[0]);
            }
            return result;
        }

        public bool GetRelPose(out UnityPose pose)
        {
            var nativePose = new Pose();
            bool result = OMWSlamWrapper.getRelPose(m_trackerPtr, ref nativePose);
            pose = new UnityPose();
            if (result)
            {
                pose.position = new UnityVector3(nativePose.position.a[0], nativePose.position.a[1], nativePose.position.a[2]);
                pose.rotation = new UnityQuaternion(nativePose.rotation.a[1], nativePose.rotation.a[2], nativePose.rotation.a[3], nativePose.rotation.a[0]);
            }
            return result;
        }

        public bool GetWorldFromTrackerTransform(out UnityMatrix4x4 transform)
        {
            transform = new UnityMatrix4x4();
            Matrix4x4 nativeTransform = new Matrix4x4();
            bool result = OMWSlamWrapper.getWorldFromTrackerTransform(m_trackerPtr, ref nativeTransform);
            if (result)
            {
                transform.m00 = nativeTransform.a[0]; transform.m01 = nativeTransform.a[1]; transform.m02 = nativeTransform.a[2]; transform.m03 = nativeTransform.a[3];
                transform.m10 = nativeTransform.a[4]; transform.m11 = nativeTransform.a[5]; transform.m12 = nativeTransform.a[6]; transform.m13 = nativeTransform.a[7];
                transform.m20 = nativeTransform.a[8]; transform.m21 = nativeTransform.a[9]; transform.m22 = nativeTransform.a[10]; transform.m23 = nativeTransform.a[11];
                transform.m30 = nativeTransform.a[12]; transform.m31 = nativeTransform.a[13]; transform.m32 = nativeTransform.a[14]; transform.m33 = nativeTransform.a[15];
            }
            return result;
        }

        public bool GetTrackerFromHeadTransform(out UnityMatrix4x4 transform)
        {
            transform = new UnityMatrix4x4();
            Matrix4x4 nativeTransform = new Matrix4x4();
            bool result = OMWSlamWrapper.getTrackerFromHeadTransform(m_trackerPtr, ref nativeTransform);
            if (result)
            {
                transform.m00 = nativeTransform.a[0]; transform.m01 = nativeTransform.a[1]; transform.m02 = nativeTransform.a[2]; transform.m03 = nativeTransform.a[3];
                transform.m10 = nativeTransform.a[4]; transform.m11 = nativeTransform.a[5]; transform.m12 = nativeTransform.a[6]; transform.m13 = nativeTransform.a[7];
                transform.m20 = nativeTransform.a[8]; transform.m21 = nativeTransform.a[9]; transform.m22 = nativeTransform.a[10]; transform.m23 = nativeTransform.a[11];
                transform.m30 = nativeTransform.a[12]; transform.m31 = nativeTransform.a[13]; transform.m32 = nativeTransform.a[14]; transform.m33 = nativeTransform.a[15];
            }
            return result;
        }

        public bool GetEyeToHeadTransform(out UnityMatrix4x4 transform)
        {
            transform = new UnityMatrix4x4();
            Matrix4x4 nativeTransform = new Matrix4x4();
            bool result = OMWSlamWrapper.getEyeToHeadTransform(m_trackerPtr, ref nativeTransform);
            if (result)
            {
                transform.m00 = nativeTransform.a[0]; transform.m01 = nativeTransform.a[1]; transform.m02 = nativeTransform.a[2]; transform.m03 = nativeTransform.a[3];
                transform.m10 = nativeTransform.a[4]; transform.m11 = nativeTransform.a[5]; transform.m12 = nativeTransform.a[6]; transform.m13 = nativeTransform.a[7];
                transform.m20 = nativeTransform.a[8]; transform.m21 = nativeTransform.a[9]; transform.m22 = nativeTransform.a[10]; transform.m23 = nativeTransform.a[11];
                transform.m30 = nativeTransform.a[12]; transform.m31 = nativeTransform.a[13]; transform.m32 = nativeTransform.a[14]; transform.m33 = nativeTransform.a[15];
            }
            return result;
        }

        public bool Init()
        {
            return OMWSlamWrapper.init(m_trackerPtr);
        }

        public bool IsReadyToUse()
        {
            return OMWSlamWrapper.isReadyToUse(m_trackerPtr);
        }

        ~Tracker()
        {
            OMWSlamWrapper.destroyTracker(m_trackerPtr);
        }
    }
}
