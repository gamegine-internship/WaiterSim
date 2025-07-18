using UnityEngine;
using System.Collections;


namespace Obi
{

    public abstract class ObiEmitterBlueprintBase : ObiActorBlueprint
    {

        public uint capacity = 1000;

        [Min(0.001f)]
        public float resolution = 1;

        [Min(0.001f)]
        public float restDensity = 1000;        /**< rest density of the material.*/

        [Header("User data diffusion")]
        public float miscibility = 0.0f;
        public Vector4 userData;                       /**< values affected by miscibility*/

        /** 
         * Returns the diameter (2 * radius) of a single particle of this material.
         */
        public float GetParticleSize(Oni.SolverParameters.Mode mode)
        {
            return 1f / (10 * Mathf.Pow(resolution, 1 / (mode == Oni.SolverParameters.Mode.Mode3D ? 3.0f : 2.0f)));
        }

        /** 
         * Returns the mass (in kilograms) of a single particle of this material.
         */
        public float GetParticleMass(Oni.SolverParameters.Mode mode)
        {
            return restDensity * Mathf.Pow(GetParticleSize(mode), mode == Oni.SolverParameters.Mode.Mode3D ? 3 : 2);
        }

        protected override IEnumerator Initialize()
        {
            ClearParticleGroups();
            m_ActiveParticleCount = 0;

            positions = new Vector3[capacity];

            yield return new CoroutineJob.ProgressInfo("ObiEmitter: done", 1);
        }
    }
}