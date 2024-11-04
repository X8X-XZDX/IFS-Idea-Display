# Point Cloud Fractals

by Acerola

This repo features tech for drawing as many particles as possible to approximate the attractor of a given iterated function system. It is not an implementation of splatting, rather the tech is game dev friendly and uses the usual rendering pipeline. It also features a real time lighting solution for the 3D fractal by implementing a brute force ambient occlusion approximation. Although, now that I'm thinking about it, I probably could've just done ssao.

This is not a production asset. Do not use it in your video games. It is a proof of concept and tech demo for how much higher end hardware can handle with even midwit implementations. Many improvements could be made.



![fractal](./Examples/flagship.png)
![fractal](./Examples/f17.png)

References: <br>
https://paulbourke.net/fractals/ifs/
https://en.wikipedia.org/wiki/Chaos_game
https://www.youtube.com/@acegikmo - Lerp smoothing is broken