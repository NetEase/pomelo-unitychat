using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SocketIOClient;
using SimpleJson;
using pomeloUnityClient;

public class LoginGUI : MonoBehaviour {
	public static string userName = "";
	public static string channel = "";
	public static JsonObject users = null;
	
	public static PomeloClient pclient = null;
	
	public Texture2D pomelo;
	public GUISkin pomeloSkin; 
	public GUIStyle pomeloStyle;
	
 	void Start() 
    {	
		pomelo = (Texture2D)Resources.Load("pomelo");
		pomeloStyle.normal.textColor = Color.black;
    }
	
	//When quit, release resource
	void Update(){
		if(Input.GetKey(KeyCode.Escape)) {
			if (pclient != null) {
				pclient.disconnect();
			}
			Application.Quit();
		}
	}
	
	//When quit, release resource
	void OnApplicationQuit(){
		if (pclient != null) {
			pclient.disconnect();
		}
	}
	
	//Login the chat application and new PomeloClient.
	void Login() {
		string url = "http://114.113.202.141:3088";
		pclient = new PomeloClient(url);
		pclient.init();
		JsonObject userMessage = new JsonObject();
		userMessage.Add("uid", userName);
		pclient.request("gate.gateHandler.queryEntry", userMessage, (data)=>{
			System.Object code = null;
			if(data.TryGetValue("code", out code)){
				if(Convert.ToInt32(code) == 500) {
					return;
				} else {
					pclient.disconnect();
					pclient = null;
					System.Object host, port;
					if (data.TryGetValue("host", out host) && data.TryGetValue("port", out port)) {
						pclient = new PomeloClient("http://" + "114.113.202.141" + ":" + port.ToString());
						pclient.init();
						Entry();
					}
				} 
			}
		});
	}
	
	//Entry chat application.
	void Entry(){
		JsonObject userMessage = new JsonObject();
		userMessage.Add("username", userName);
		userMessage.Add("rid", channel);
		if (pclient != null) {
			pclient.request("connector.entryHandler.enter", userMessage, (data)=>{
				users = data;
				Application.LoadLevel(Application.loadedLevel + 1);
			});
		}
	}
	
	void OnGUI(){
		GUI.skin = pomeloSkin;
		GUI.color = Color.yellow;
		GUI.enabled = true;	
		GUI.Label(new Rect(160, 50, pomelo.width, pomelo.height), pomelo);
		
		GUI.Label(new Rect(75, 350, 50, 20), "name:", pomeloStyle);
		userName = GUI.TextField(new Rect(125, 350, 90, 20), userName);
		GUI.Label(new Rect(225, 350, 55, 20), "channel:", pomeloStyle);
		channel = GUI.TextField(new Rect(280, 350, 100, 20), channel);
		
		if (GUI.Button(new Rect(410, 350, 70, 20), "OK")) {
			if (pclient == null) {
				Login();
			}
		}	
	}

 }