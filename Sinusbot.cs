﻿using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuchByte.SinusBotPlugin
{
    public class Sinusbot
    {
        private readonly string _apiString = "/api/v1";
        private readonly string _bearerToken = "";
        private readonly string _botId = "";
        private readonly string _url = "";
        private readonly bool _loggedIn = false;

        public bool LoggedIn { get { return this._loggedIn; } }

        public Sinusbot(string url = "", string username = "", string password = "")
        {
            this._url = url;
            try
            {
                this._botId = ApiCallObject("/botId", null, HttpRequestMethod.GET)["defaultBotId"];
            } catch { }

            Dictionary<string, string> loginObj = ApiCallObject("/bot/login", new Dictionary<string, string>
            {
                { "username", username },
                { "password", password },
                { "botId", this._botId }
            }, HttpRequestMethod.POST);

            if (loginObj != null)
            {
                this._bearerToken = loginObj["token"];
                this._loggedIn = true;
            }
        }

        public string GetInstanceId(string name)
        {
            try
            {
                foreach (JObject instanceObject in this.GetBotInstances())
                {
                    foreach (KeyValuePair<string, JToken> arg in instanceObject)
                    {
                        if (arg.Key.Equals("name") && arg.Value.ToString().Equals(name))
                        {
                            return instanceObject["uuid"].ToString();
                        }
                    }
                }
            } catch { }
            return "";
        }

        public string GetFileId(string title)
        {
            try
            {
                foreach (JObject fileObject in this.GetFileList())
                {
                    foreach (KeyValuePair<string, JToken> arg in fileObject)
                    {
                        if (arg.Key.Equals("title") && arg.Value.ToString().Equals(title))
                        {
                            return fileObject["uuid"].ToString();
                        }
                    }
                }
            } catch { }
            return "";
        }

        public string GetInstanceName(string id)
        {
            try
            {
                foreach (JObject instanceObject in this.GetBotInstances())
                {
                    foreach (KeyValuePair<string, JToken> arg in instanceObject)
                    {
                        if (arg.Key.Equals("uuid") && arg.Value.ToString().Equals(id))
                        {
                            return instanceObject["name"].ToString();
                        }
                    }
                }
            } catch { }
            return "";
        }

        public string GetFileTitle(string id)
        {
            try
            {
                foreach (JObject instanceObject in this.GetFileList())
                {
                    foreach (KeyValuePair<string, JToken> arg in instanceObject)
                    {
                        if (arg.Key.Equals("uuid") && arg.Value.ToString().Equals(id))
                        {
                            return instanceObject["title"].ToString();
                        }
                    }
                }
            } catch { }
            return "";
        }

        public void SetVolume(string instanceId, int volume)
        {
            if (!this._loggedIn)
                return;
            this.ApiCallObject("/bot/i/" + instanceId + "/volume/set/" + volume, null, HttpRequestMethod.POST);
        }

        public void IncreaseVolume(string instanceId)
        {
            if (!this._loggedIn)
                return;
            this.ApiCallObject("/bot/i/" + instanceId + "/volume/up", null, HttpRequestMethod.POST);
        }
        public void DecreaseVolume(string instanceId)
        {
            if (!this._loggedIn)
                return;
            this.ApiCallObject("/bot/i/" + instanceId + "/volume/down", null, HttpRequestMethod.POST);
        }

        public void StopPlayback(string instanceId)
        {
            if (!this._loggedIn)
                return;
            this.ApiCallObject("/bot/i/" + instanceId + "/stop", null, HttpRequestMethod.POST);
        }

        public void PlayFile(string instanceId, string fileId)
        {
            if (!this._loggedIn)
                return;
            this.ApiCallObject("/bot/i/" + instanceId + "/play/byId/" + fileId, null, HttpRequestMethod.POST);
        }

        public List<JObject> GetFileList()
        {
            if (!this._loggedIn)
                return null;
            List<JObject> files = new List<JObject>();
            foreach (string instanceJson in this.ApiCallArray("/bot/files", null, HttpRequestMethod.GET))
            {
                files.Add(JObject.Parse(instanceJson));
            }

            return files;
        }

        public List<JObject> GetBotInstances()
        {
            if (!this._loggedIn)
                return null;
            List<JObject> instances = new List<JObject>();
            foreach (string instanceJson in this.ApiCallArray("/bot/instances", null, HttpRequestMethod.GET))
            {
                instances.Add(JObject.Parse(instanceJson));
            }

            return instances;
        }

        private IRestResponse ApiCall(string api, Dictionary<string, string> args, HttpRequestMethod httpRequestMethod)
        {
            var client = new RestClient(this._url);
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", this._bearerToken));
            var request = new RestRequest(this._apiString + api);
            if (args != null)
            {
                foreach (KeyValuePair<string, string> arg in args)
                {
                    request.AddParameter(arg.Key, arg.Value);
                }
            }
            IRestResponse response = null;
            switch (httpRequestMethod)
            {
                case HttpRequestMethod.GET:
                    response = client.Get(request);
                    break;
                case HttpRequestMethod.POST:
                    response = client.Post(request);
                    break;
            }
            return response;
        }

        private Dictionary<string, string> ApiCallObject(string api, Dictionary<string, string> args, HttpRequestMethod httpRequestMethod)
        {
            Dictionary<string, string> jsonObject;
            var response = this.ApiCall(api, args, httpRequestMethod);
            JsonDeserializer deserial = new JsonDeserializer();
            try
            {
                jsonObject = deserial.Deserialize<Dictionary<string, string>>(response);
            }
            catch
            {
                jsonObject = null;
            }
            return jsonObject;
        }

        private List<string> ApiCallArray(string api, Dictionary<string, string> args, HttpRequestMethod httpRequestMethod)
        {
            List<string> jsonArray;
            var response = this.ApiCall(api, args, httpRequestMethod);
            JsonDeserializer deserial = new JsonDeserializer();
            try
            {
                jsonArray = deserial.Deserialize<List<string>>(response);
            }
            catch
            {
                jsonArray = null;
            }
            return jsonArray;
        }
    }

    public enum HttpRequestMethod
    {
        GET,
        POST
    }
}