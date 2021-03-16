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

    [HideInInspector] public Transform level;

    Scene currentScene;
    Scene predictionScene;

    PhysicsScene currentPhysicsScene;
    PhysicsScene predictionPhysicsScene;

    List<GameObject> similuatedObstacles = new List<GameObject>();
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
        foreach (Transform t in level.transform) {
            if (t.GetComponent<PlayerMotor>())
                continue;

            if (t.gameObject.GetComponent<Collider>() != null) {
                GameObject simT = Instantiate(t.gameObject);
                simT.transform.position = t.position;
                simT.transform.rotation = t.rotation;
                Renderer simR = simT.GetComponent<Renderer>();
                if (simR) {
                    simR.enabled = false;
                }
                SceneManager.MoveGameObjectToScene(simT, predictionScene);
                similuatedObstacles.Add(simT);
            }
        }
    }


    private void FixedUpdate() {
		if(currentPhysicsScene.IsValid()) {
            currentPhysicsScene.Simulate(Time.fixedDeltaTime);
		}
	}

    public void Predict(GameObject subject, Vector3 currentPosition, Vector3 force) {
        if (currentPhysicsScene.IsValid() && predictionPhysicsScene.IsValid()) {
            if (similuatedObject == null) {
                similuatedObject = Instantiate(subject);
                similuatedObject.GetComponent<Collider>().material = null;
                SceneManager.MoveGameObjectToScene(similuatedObject, predictionScene);
            }

            similuatedObject.transform.position = currentPosition;
            similuatedObject.GetComponent<PlayerMotor>().Move(force*2.5f);
            lineRenderer.positionCount = 0;
            lineRenderer.positionCount = maxIterations;


            for (int i = 0; i < maxIterations; i++) {
                if (i>0) {
                    predictionPhysicsScene.Simulate(steps);
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