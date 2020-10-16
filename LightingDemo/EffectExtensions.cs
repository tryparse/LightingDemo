using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightingDemo
{
    static class EffectExtensions
    {
        public static bool TrySetParameter(this Effect effect, string parameterName, Texture value)
        {
            if (effect.Parameters.Any(x => x.Name == parameterName))
            {
                effect.Parameters[parameterName].SetValue(value);

                return true;
            }

            return false;
        }

        public static bool TrySetParameter(this Effect effect, string parameterName, Matrix value)
        {
            if (effect.Parameters.Any(x => x.Name == parameterName))
            {
                effect.Parameters[parameterName].SetValue(value);

                return true;
            }

            return false;
        }

        public static bool TrySetParameter(this Effect effect, string parameterName, Vector4 value)
        {
            if (effect.Parameters.Any(x => x.Name == parameterName))
            {
                effect.Parameters[parameterName].SetValue(value);

                return true;
            }

            return false;
        }

        public static bool TrySetParameter(this Effect effect, string parameterName, Vector2 value)
        {
            if (effect.Parameters.Any(x => x.Name == parameterName))
            {
                effect.Parameters[parameterName].SetValue(value);

                return true;
            }

            return false;
        }

        public static bool TrySetParameter(this Effect effect, string parameterName, float value)
        {
            if (effect.Parameters.Any(x => x.Name == parameterName))
            {
                effect.Parameters[parameterName].SetValue(value);

                return true;
            }

            return false;
        }
    }
}
