// Геометрия гексагонального поля для отрисовки в SVG (architecture.md, раздел 7).
// Правил игры здесь нет: только перевод координат клетки в пиксели.

export interface Hex {
  q: number
  r: number
}

export interface Point {
  x: number
  y: number
}

// Центр клетки в пикселях для ориентации острой вершиной вверх; size задаёт радиус клетки
export function hexToPixel(hex: Hex, size: number): Point {
  return {
    x: size * Math.sqrt(3) * (hex.q + hex.r / 2),
    y: size * 1.5 * hex.r,
  }
}
