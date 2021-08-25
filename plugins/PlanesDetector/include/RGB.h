#pragma once
// TODO: we don't need it for Elektronik so it can be removed.

// Red-Green-Blue color.
class RGB
{
public:
    inline RGB() : r(0), g(0), b(0)
    { }

    inline RGB(unsigned char red, unsigned char green, unsigned char blue) : r(red), g(green), b(blue)
    { }

    unsigned char r;
    unsigned char g;
    unsigned char b;
};
