#ifndef Vec3_h_
#define Vec3_h_

#include <cmath>

// 3D-vector
template <typename T>
struct Vector3 {
    // Members.
    union {
        struct {
            T x, y, z;
        };
        T D[3];
    };

    // Constructors.
    inline Vector3() :
        x(0), y(0), z(0) {}
    inline Vector3(T _x, T _y, T _z) :
        x(_x), y(_y), z(_z) {}

    inline bool nonNull()
        {return x != 0 || y != 0 || z != 0;}

    // Access component.
    inline T& operator[](unsigned int i)
        {return D[i];}
    inline const T& operator[](unsigned int i) const
        {return D[i];}

    // Operations.
    inline Vector3 operator+(const Vector3& p) const
        {return Vector3(x + p.x, y + p.y, z + p.z);}
    inline Vector3 operator-(const Vector3& p) const
        {return Vector3(x - p.x, y - p.y, z - p.z);}

    // Multiply/Divide by component.
    inline Vector3 cmul(const Vector3& p) const
        {return Vector3(x * p.x, y * p.y, z * p.z);}
    inline Vector3 cdiv(const Vector3& p) const
        {return Vector3(x / p.x, y / p.y, z / p.z);}

    // External product.
    inline Vector3 operator*(T t) const
        {return Vector3(x * t, y * t, z * t);}
    inline Vector3 operator/(T t) const
        {return Vector3(x / t, y / t, z / t);}

    inline Vector3& operator+=(const Vector3& r)
        {x += r.x; y += r.y; z += r.z; return *this;}
    inline Vector3& operator-=(const Vector3& r)
        {x -= r.x; y -= r.y; z -= r.z; return *this;}

    inline Vector3& operator*=(T t)
        {x *= t; y *= t; z *= t; return *this;}
    inline Vector3& operator/=(T t)
        {x /= t; y /= t; z /= t; return *this;}

    // Inner/dot product
    inline T operator*(const Vector3& p) const
        {return x * p.x + y * p.y + z * p.z;}

    inline T norm() const
        {return std::sqrt(x*x + y*y + z*z);}
    inline T normSquared() const
        {return x*x + y*y + z*z;}

    inline void normalize()
        {*this /= norm();}
    inline Vector3 normalized() const
        {return *this / norm();}

    inline T distance(const Vector3& r) const
        {return (*this - r).norm();}
    inline T squareDistance(const Vector3& r) const
        {return (*this - r).normSquared();}

    // Cross product
    Vector3 operator^(const Vector3& r) const
    {
        return Vector3(
                y * r.z - z * r.y,
                z * r.x - x * r.z,
                x * r.y - y * r.x
                );
    }

    void max(const Vector3& p)
    {
        x = p.x > x ? p.x : x;
        y = p.y > y ? p.y : y;
        z = p.z > z ? p.z : z;
    }

    void min(const Vector3& p)
    {
        x = p.x < x ? p.x : x;
        y = p.y < y ? p.y : y;
        z = p.z < z ? p.z : z;
    }
};

template <typename T>
inline Vector3<T> operator*(T t, const Vector3<T>& v) {
    return Vector3<T>(v.x * t, v.y * t, v.z * t);
}

typedef Vector3<double> Vec3d;

#endif
