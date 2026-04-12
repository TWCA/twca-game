// Written using p5.js 2.2.0
// Hosted at https://openprocessing.org/sketch/2853523

async function setup() {
	palette = await loadImage("fullPalette.png");
	palette.loadPixels();

	const dayPalette = parsePalette(palette, 0, 45);
	const dawnPalette = parsePalette(palette, 48, 45);
	const nightPalette = parsePalette(palette, 96, 45);

	const depth = 6; // size of palette
	const fromPalette = dayPalette;
	const toPalette = dawnPalette; // dayPalette or dawnPalette or nightPalette

	const levels = 1 << depth;
	const squaresWide = 1 << Math.ceil(depth / 2);
	const squaresHigh = 1 << Math.floor(depth / 2);
	const width = levels * squaresWide;
	const height = levels * squaresHigh;

	createCanvas(width, height);
	loadPixels();

	for (let x = 0; x < width; x++) {
		for (let y = 0; y < width; y++) {
			const r = x % levels;
			const g = y % levels;
			const b = Math.floor(x / levels) + Math.floor(y / levels) * squaresWide;

			const index = nearest(fromPalette, r, g, b, levels);

			const offset = (x + y * width) * 4;
			pixels[offset + 0] = toPalette[index][0];
			pixels[offset + 1] = toPalette[index][1];
			pixels[offset + 2] = toPalette[index][2];
			pixels[offset + 3] = 255;
		}
	}

	updatePixels();
}

// find nearest colour in the palette
function nearest(palette, r, g, b, levels) {
	r = (r / (levels - 1)) * 255;
	g = (g / (levels - 1)) * 255;
	b = (b / (levels - 1)) * 255;

	let minDist = Infinity;
	let minIndex;
	for (const [index, color] of palette.entries()) {
		const dist = Math.hypot(r - color[0], g - color[1], b - color[2]);

		if (dist < minDist) {
			minDist = dist;
			minIndex = index;
		}
	}

	return minIndex;
}

function parsePalette(img, offset, size) {
	const palette = [];

	for (let i = offset; i < offset + size; i++) {
		const red = img.pixels[i * 4 + 0];
		const green = img.pixels[i * 4 + 1];
		const blue = img.pixels[i * 4 + 2];
		palette.push([red, green, blue]);
	}

	return palette;
}
