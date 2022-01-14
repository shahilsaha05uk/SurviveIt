using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SelectionBase]
public class MonsterScript : EnemyMethods
{
    public enum States { WANDER, ATTACK, DEAD };


        private void OnEnable()
        {
            EnemySetup();
            anim = GetComponent<Animator>();
            destinationReached = false;
            enemyState = EnemyState.IDLE;


            StartCoroutine(EnemyAction());
            StartCoroutine(base.UpdateTargetList());
            StartCoroutine(base.EnemyChangePath());

        }
        private void FixedUpdate()
        {
            if (GetComponent<HealthScript>().Health() <= 0)
            {
                enemyState = EnemyState.DEAD;
                EnemyAnimState();

                DestroyScript.DestroyElement(this.gameObject);
            }
        }

        public override IEnumerator UpdateTargetList()
        {
            return base.UpdateTargetList();
        }
        public override IEnumerator EnemyChangePath()
        {
            return base.EnemyChangePath();
        }
        public override void RemoveTargetFromTheList(GameObject destroyedTarget)
        {
            base.RemoveTargetFromTheList(destroyedTarget);
        }
        public override Transform NextTarget()
        {
            return base.NextTarget();
        }

        public override void EnemyAnimState()
        {
            base.EnemyAnimState();
        }
        public override IEnumerator EnemyAttack()
        {
            return base.EnemyAttack();
        }
        public override IEnumerator EnemyIdle()
        {
            return base.EnemyIdle();
        }
        public override IEnumerator EnemyAction()
        {
            return base.EnemyAction();
        }
        public override IEnumerator EnemyWalk()
        {
            return base.EnemyWalk();
        }
        public override void EnemySetup()
        {
            base.EnemySetup();
        }
}
