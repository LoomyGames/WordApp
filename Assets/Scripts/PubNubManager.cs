using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PubNubAPI;
using System;

public class WordData
{
    public string[] wordStorage = new string[20];
}
public class PubNubManager
{
    PubNub pubnub;
    string cipherKey = "";
    string channel = "Word4Word";
    string NEW_WORDS_FILE = System.IO.Path.Combine(Application.persistentDataPath, "New_Words.txt");

    public void Init()
    {
        Debug.Log("Starting");
        PNConfiguration pnConfiguration = new PNConfiguration();
        pnConfiguration.SubscribeKey = "sub-c-29000a87-5fb6-48b1-b999-4aa999af9bc2";
        pnConfiguration.PublishKey = "pub-c-27b92812-ce75-4fb5-b940-558078cbbbf0";
        pnConfiguration.SecretKey = "sec-c-NmExOGNlMmEtNGFmYy00Y2ZjLWIzZDgtMDJkYTE2ZTU0YzBh";

        pnConfiguration.CipherKey = cipherKey;
        pnConfiguration.LogVerbosity = PNLogVerbosity.BODY;
        pnConfiguration.PresenceTimeout = 120;
        pnConfiguration.PresenceInterval = 60;
        pnConfiguration.HeartbeatNotificationOption = PNHeartbeatNotificationOption.All;

        //TODO: remove
        pnConfiguration.UserId = "PubNubUnityExample_APP";
        Debug.Log("PNConfiguration");
        pubnub = new PubNub(pnConfiguration);

        pubnub.SubscribeCallback += SubscribeCallbackHandler;
        pubnub.Subscribe().Channels(new List<string>() { channel }).Execute();
    }

    private void SubscribeCallbackHandler(object sender, EventArgs e)
    {
        Debug.Log("SubscribeCallbackHandler Event handler");
        SubscribeEventEventArgs mea = e as SubscribeEventEventArgs;

        if (mea.Status != null)
        {
            switch (mea.Status.Category)
            {
                case PNStatusCategory.PNConnectedCategory:
                    //PrintStatus(mea.Status);
                    break;
                case PNStatusCategory.PNUnexpectedDisconnectCategory:
                case PNStatusCategory.PNTimeoutCategory:
                    pubnub.Reconnect();
                    pubnub.CleanUp();
                    break;
            }
        }
        else
        {
            Debug.Log("mea.Status null" + e.GetType().ToString() + mea.GetType().ToString());
        }
        if (mea.MessageResult != null)
        {
            Debug.Log("In Example, SubscribeCallback in message" + mea.MessageResult.Channel + " : " + mea.MessageResult.Payload);
            SaveWords(mea.MessageResult.Payload);
        }
        if (mea.PresenceEventResult != null)
        {
            Debug.Log("In Example, SubscribeCallback in presence" + mea.PresenceEventResult.Channel + mea.PresenceEventResult.Occupancy + mea.PresenceEventResult.Event + mea.PresenceEventResult.State);
        }
        if (mea.SignalEventResult != null)
        {
            Debug.Log("In Example, SubscribeCallback in SignalEventResult" + mea.SignalEventResult.Channel + mea.SignalEventResult.Payload);
        }
    }

    private void SaveWords(object json)
    {
        Debug.Log("--SaveWords--" + NEW_WORDS_FILE);

        // saved to file
        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(NEW_WORDS_FILE))
        {
            writer.Write(json);
        }

        // You can use live publised data
        //WordData data = JsonUtility.FromJson<WordData>(json.ToString());
    }

    /// <summary>
    /// Get last published words.
    /// </summary>
    /// <returns>WordData if published and null if no data has been published yet.</returns>
    public WordData GetNewWords()
    {
        if (!System.IO.File.Exists(NEW_WORDS_FILE)) 
            return null;

        return JsonUtility.FromJson<WordData>(System.IO.File.ReadAllText(NEW_WORDS_FILE));
    }

}
