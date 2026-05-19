using System;
using UnityEngine;

[Serializable]
public class MatrixDto
{
    public float m00;
    public float m10;
    public float m20;
    public float m30;

    public float m01;
    public float m11;
    public float m21;
    public float m31;

    public float m02;
    public float m12;
    public float m22;
    public float m32;

    public float m03;
    public float m13;
    public float m23;
    public float m33;

    public MatrixDto() { }

    public MatrixDto(Matrix4x4 m)
    {
        m00 = m.m00; m10 = m.m10; m20 = m.m20; m30 = m.m30;
        m01 = m.m01; m11 = m.m11; m21 = m.m21; m31 = m.m31;
        m02 = m.m02; m12 = m.m12; m22 = m.m22; m32 = m.m32;
        m03 = m.m03; m13 = m.m13; m23 = m.m23; m33 = m.m33;
    }

    public Matrix4x4 ToMatrix4X4()
    {
        var m = new Matrix4x4
        {
            m00 = m00,
            m10 = m10,
            m20 = m20,
            m30 = m30,
            m01 = m01,
            m11 = m11,
            m21 = m21,
            m31 = m31,
            m02 = m02,
            m12 = m12,
            m22 = m22,
            m32 = m32,
            m03 = m03,
            m13 = m13,
            m23 = m23,
            m33 = m33
        };

        return m;
    }
}