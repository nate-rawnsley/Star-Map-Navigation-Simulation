using UnityEngine;

//A library of easing functions, from my Modes of Motion assessment. (sourced from easings.net)
public class Easing {
    public static float Linear(float t) {
        return t;
    }

    public static float PP(float t) {
        return Mathn.PingPong(t, 0.5f);
    }

    public class Quadratic {
        public static float In(float t) {
            return t * t;
        }
        public static float Out(float t) {
            return t * (2f - t);
        }
        public static float InOut(float t) {
            if ((t *= 2f) < 1.0f) return 0.5f * t * t;
            return -0.5f * ((t -= 1.0f) * (t - 2f) - 1f);
        }
        public static float Bezier(float t, float c) {
            return c * 2 * t * (1 - t) + t * t;
        }
    }

    public class Sine {
        public static float In(float t) {
            return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
        }
        public static float Out(float t) {
            return Mathf.Sin(t * Mathf.PI * 0.5f);
        }
        public static float InOut(float t) {
            return -(Mathf.Cos(t * Mathf.PI) - 1) * 0.5f;
        }
    }

    public class Quart {
        public static float In(float t) {
            return t * t * t * t;
        }
        public static float Out(float t) {
            return 1 - Mathf.Pow(1 - t, 4);
        }
        public static float InOut(float t) {
            return t < 0.5 ? 8 * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 4) / 2;
        }
    }

    public class Quint {
        public static float In(float t) {
            return t * t * t * t * t;
        }
        public static float Out(float t) {
            return 1 - Mathf.Pow(1 - t, 5);
        }
        public static float InOut(float t) {
            return t < 0.5 ? 16 * t * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 5) / 2;
        }
    }

    public class Circ {
        public static float In(float t) {
            return 1 - Mathf.Sqrt(1 - t * t);
        }
        public static float Out(float t) {
            return Mathf.Sqrt(1 - Mathf.Pow(t - 1, 2));
        }
        public static float InOut(float t) {
            return t < 0.5
              ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * t, 2))) / 2
              : (Mathf.Sqrt(1 - Mathf.Pow(-2 * t + 2, 2)) + 1) / 2;
        }
    }

    public class Back {
        public static float In(float t) {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;

            return c3 * t * t * t - c1 * t * t;
        }
        public static float Out(float t) {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;

            return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
        }
        public static float InOut(float t) {
            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;

            return t < 0.5
              ? (Mathf.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2)) / 2
              : (Mathf.Pow(2 * t - 2, 2) * ((c2 + 1) * (t * 2 - 2) + c2) + 2) / 2;
        }
    }

    public class Bounce {
        public static float In(float t) {
            const float c4 = (2 * Mathf.PI) / 3;

            return t == 0
              ? 0
              : t == 1
              ? 1
              : -Mathf.Pow(2, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * c4);
        }
        public static float Out(float t) {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (t < 1 / d1) {
                return n1 * t * t;
            } else if (t < 2 / d1) {
                return n1 * (t -= 1.5f / d1) * t + 0.75f;
            } else if (t < 2.5 / d1) {
                return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            } else {
                return n1 * (t -= 2.625f / d1) * t + 0.984375f;
            }
        }
        public static float InOut(float t) {
            return t < 0.5
                ? (1 - Out(1 - 2 * t)) / 2
                : (1 + Out(2 * t - 1)) / 2;
        }
    }

    public class Elastic {
        public static float In(float t) {
            const float c4 = (2 * Mathf.PI) / 3;

            return t == 0
              ? 0
              : t == 1
              ? 1
              : -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10f - 10.75f) * c4);
        }
        public static float Out(float t) {
            const float c4 = (2 * Mathf.PI) / 3f;

            return t == 0
              ? 0
              : t == 1
              ? 1
              : Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1;
        }
        public static float InOut(float t) {
            const float c5 = (2 * Mathf.PI) / 4.5f;

            return t == 0
              ? 0
              : t == 1
              ? 1
              : t < 0.5
              ? -(Mathf.Pow(2, 20 * t - 10) * Mathf.Sin((20f * t - 11.125f) * c5)) / 2
              : (Mathf.Pow(2, -20 * t + 10) * Mathf.Sin((20f * t - 11.125f) * c5)) / 2 + 1;
        }
    }

    public static float LerpSwitch(float time, int value) {
        switch (value) {
            case -1:
                return Linear(time);
            case 0:
                return Quadratic.In(time);
            case 1:
                return Quadratic.Out(time);
            case 2:
                return Quadratic.InOut(time);
            case 3:
                return Sine.In(time);
            case 4:
                return Sine.Out(time);
            case 5:
                return Sine.InOut(time);
            case 6:
                return Quart.In(time);
            case 7:
                return Quart.Out(time);
            case 8:
                return Quart.InOut(time);
            case 9:
                return Quint.In(time);
            case 10:
                return Quint.Out(time);
            case 11:
                return Quint.InOut(time);
            case 12:
                return Circ.In(time);
            case 13:
                return Circ.Out(time);
            case 14:
                return Circ.InOut(time);
            case 15:
                return Back.In(time);
            case 16:
                return Back.Out(time);
            case 17:
                return Back.InOut(time);
            case 18:
                return Bounce.In(time);
            case 19:
                return Bounce.Out(time);
            case 20:
                return Bounce.InOut(time);
            case 21:
                return Elastic.In(time);
            case 22:
                return Elastic.Out(time);
            case 23:
                return Elastic.InOut(time);
            default:
                return 0f;
        }
    }
}
