using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PredictionManager : MonoBehaviour
{
    public int maxIterations = 100;
    public float steps = 0.025f;
    public LineRenderer lineRenderer;
    public float lineYOffset;
    public float similuatedBallScale = .85f;

    [HideInInspector] public Transform level;
    [HideInInspector] public bool doPredict = true;

    Scene currentScene;
    Scene predictionScene;

    PhysicsScene currentPhysicsScene;
    PhysicsScene predictionPhysicsScene;

    List<GameObject> similuatedObstacles = new List<GameObject>();
    List<GameObject> similuatedDynamicObstacles = new List<GameObject>();
    List<GameObject> dynamicObstacles = new List<GameObject>();
    GameObject similuatedObject;

    private void Awake() {
        Physics.autoSimulation = false;

        currentScene = SceneManager.GetActiveScene();
        currentPhysicsScene = currentScene.GetPhysicsScene();

        CreateSceneParameters parameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        predictionScene = SceneManager.CreateScene("Prediction", parameters);
        predictionPhysicsScene = predictionScene.GetPhysicsScene();

        lineRenderer.positionCount = 0;
    }


    public void UpdateLevel() {
        ClearPhysicScene();

        foreach (Transform t in level.transform) {
            if (t.GetComponent<PlayerMotor>())
                continue;

            if (t.gameObject.GetComponent<Collider>() != null
                && t.gameObject.activeInHierarchy) {
                GameObject simT = Instantiate(t.gameObject);

                // if is ball, add it to special list
                BallMotor motor = simT.GetComponent<BallMotor>();
                if (motor) {
                    dynamicObstacles.Add(t.gameObject);
                    similuatedDynamicObstacles.Add(simT);

                    DestroyImmediate(motor);
                    if (simT.GetComponent<Rigidbody>()) {
                        DestroyImmediate(simT.GetComponent<Rigidbody>());
                    }
				}


                simT.transform.position = t.position;
                simT.transform.rotation = t.rotation;
                simT.transform.localScale = t.lossyScale;
                Renderer simR = simT.GetComponent<Renderer>();
                if (simR) {
                    simR.enabled = false;
                }
				foreach (Renderer simChildR in simT.GetComponentsInChildren<Renderer>())
				{
                    simChildR.enabled = false;
                }
                SceneManager.MoveGameObjectToScene(simT, predictionScene);
                similuatedObstacles.Add(simT);
            }
        }
    }

    // delete existing objects in physics scene
    private void ClearPhysicScene()
	{
		similuatedObstacles.ForEach(obj => {
            Destroy(obj);
        });
        similuatedDynamicObstacles.ForEach(obj => {
            Destroy(obj);
        });

        similuatedObstacles = new List<GameObject>();
        similuatedDynamicObstacles = new List<GameObject>();
        dynamicObstacles = new List<GameObject>();
    }

    public void UpdateDynamicObsticles() {
		for (int i = 0; i < dynamicObstacles.Count; i++) {
            GameObject originalObj = dynamicObstacles[i];
            GameObject similuatedObj = similuatedDynamicObstacles[i];

            // TODO: If health is less than 1 then skip destory and stop updating

            similuatedObj.transform.position = originalObj.transform.position;
            similuatedObj.transform.rotation = originalObj.transform.rotation;
        }
    }


    private void FixedUpdate() {
		if(currentPhysicsScene.IsValid()) {
            currentPhysicsScene.Simulate(Time.fixedDeltaTime);
		}
	}

    public void Predict(GameObject subject, Vector3 currentPosition, Vector3 force) {
        if (doPredict && currentPhysicsScene.IsValid() && predictionPhysicsScene.IsValid()) {

            UpdateDynamicObsticles();

            if (similuatedObject == null) {
                similuatedObject = Instantiate(subject);
                similuatedObject.GetComponent<Collider>().material = null;
                SceneManager.MoveGameObjectToScene(similuatedObject, predictionScene);
            }

            similuatedObject.transform.position = currentPosition;
            similuatedObject.transform.localScale *= similuatedBallScale;
            similuatedObject.GetComponent<PlayerMotor>().frameCountBeforeSimulationBreak = 1;
            similuatedObject.GetComponent<PlayerMotor>().Move(force*2f);
            lineRenderer.positionCount = 0;
            lineRenderer.positionCount = maxIterations;


            for (int i = 0; i < maxIterations; i++) {
                if (i>0) {
                    predictionPhysicsScene.Simulate(steps);
                    similuatedObject.GetComponent<PlayerMotor>().frameCountBeforeSimulationBreak--;
                    similuatedObject.GetComponent<PlayerMotor>().ApplyGravity();

                    if (similuatedObject.GetComponent<PlayerMotor>().breakSimulation) {
                        lineRenderer.positionCount = i;
                        break;
					}
				}
                Vector3 simPosition = similuatedObject.transform.position;
                simPosition.y += lineYOffset;
                lineRenderer.SetPosition(i, simPosition);
            }

            Destroy(similuatedObject);
        }
    }

    public void ClearPrediction() {
        lineRenderer.positionCount = 0;
    }
}
