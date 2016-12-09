#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
#include <DHT.h>
#include <stdlib.h>
#define DHTTYPE DHT22
#define DHTPIN   2
#define READLED  4
#define ERRORLED 5
#define WIFICOUNT 5

// Wi-Fi info - IDs and passwords
String ssids[WIFICOUNT]  = { "INTERTECH_GUEST", "HomeSite1", "HomeSite2", "RemoteSite1", "RemoteSite2" };
String pwds[WIFICOUNT] = { "GuestPass", "HomePass1", "HomePass2", "RemotePass1", "RemotePass2" };
const int wifinum = 5;
int retries = 15;

// Target site URL where we will post the data
char* host = "MyWebSite.azurewebsites.net";
const int postPort = 80;

// this device's ID as tracked in the database
String deviceId = "1";

// initialize the ESP8266 web server to respond on port 80
ESP8266WebServer server(80);

// Initialize DHT sensor 
DHT dht(DHTPIN, DHTTYPE, 11); // 11 works fine for ESP8266
 
float humidity, temp_f, light;  // Values read from sensors
String webString = "";     // String to display

// Generally, you should use "unsigned long" for variables that hold time
unsigned long previousMillis = 0;        // will store last temp was read
const long interval = 2000;              // interval at which to read sensor
bool connected = true;
const long postDuration = 60000;
unsigned long lastPost = 0;


/*
 * Method to convert floats/measurements to strings
 * Makes some assumptions on length
 */
String floatAsString(float val) {
  char temp[6];
  dtostrf(val, 6, 2, temp);
  String fas = String(temp);
  fas.trim();
  return fas;
}


/*
 * Gets the readings/measurements and stores them to global variables
 */
void getReadings() {
  Serial.println(" - Getting readings...");

  // Wait at least 2 seconds seconds between measurements.
  // if the difference between the current time and last time you read
  // the sensor is bigger than the interval you set, read the sensor
  // Works better than delay for things happening elsewhere also
  unsigned long currentMillis = millis();

  if(currentMillis - previousMillis >= interval) {
    // save the last time you read the sensor 
    previousMillis = currentMillis;   

    // Reading temperature for humidity takes about 250 milliseconds!
    // Sensor readings may also be up to 2 seconds 'old' (it's a very slow sensor)
    humidity = dht.readHumidity();          // Read humidity (percent)
    temp_f = dht.readTemperature(true);     // Read temperature as Fahrenheit

    // Check if any reads failed and exit early (to try again).
    if (isnan(humidity) || isnan(temp_f)) {
      Serial.println("Failed to read from DHT sensor!");
      return;
    }
  }

  // next, get light level
  light = ((float)analogRead(A0) / (float)1024) * 100;  

  // show readings in serial
  Serial.println(" - Temperature: " + floatAsString(temp_f) + " F");
  Serial.println(" - Humidity: " + floatAsString(humidity) + " %");
  Serial.println(" - Light: " + floatAsString(light) + " %");    
}


/*
 * Handles root web request by building a simple
 * web page containing current measurements
 */
void handle_root() {
  Serial.println("Handle root - start");

  digitalWrite(READLED, HIGH);
  getReadings();

  Serial.println(" - Building request");

  webString = "<html><head><title>Temperature and humidity via ESP8266 and DHT22 (v2)</title></head><body>";  
  webString = webString + "<h3>Environment Watch <br> Temperature, humidity, and light levels via ESP8266</h3>";
  webString = webString + "<p></br>";
  webString = webString + "Device ID: " + deviceId + "</br>";
  webString = webString + "Temp: " + floatAsString(temp_f) + " F</br>";
  webString = webString + "Humidity: " + floatAsString(humidity) + "%</br>";
  webString = webString + "Light: " + floatAsString(light) + "%</br>";
  webString = webString + "</br></p>";  
  webString = webString + "<p>Refresh your browser to see the most recent readings.</p>";
  webString = webString + "</body></html>";

  server.send(200, "text/html", webString);
  Serial.println(" - Request sent");

  delay(500);
  digitalWrite(READLED, LOW);
  Serial.println("Handle root - finish");
  Serial.println("");
}


/*
 * Method to handle the task of getting readings as 
 * well as posting them to an external service
 */
void post_data() {
  Serial.println("Post Data - start ");

  digitalWrite(READLED, HIGH);
  digitalWrite(ERRORLED, HIGH);

  getReadings();

  Serial.print(" - Connecting to ");
  Serial.println(host);

  Serial.print(" - On port ");
  Serial.println(postPort);  
  
  // Use WiFiClient class to create TCP connections
  WiFiClient client;
  if (!client.connect(host, postPort)) {
    Serial.println(" - Connection to host failed!");
    return;
  }

  digitalWrite(ERRORLED, LOW);

  // We now create a URI for the request
  String url = String("/Home/PostData") +
    String("?id=") + deviceId +
    String("&ip=") + WiFi.localIP().toString() +
    String("&temp=") + floatAsString(temp_f) +
    String("&humidity=") + floatAsString(humidity) +
    String("&light=") + floatAsString(light);
  Serial.println(" - Requesting URL: ");
  Serial.print("     ");
  Serial.println(url);
  
  // This will send the request to the server
  client.print(String("GET ") + url + " HTTP/1.1\r\n" +
               "Host: " + host + "\r\n" + 
               "Connection: close\r\n\r\n");
  delay(500);

  Serial.println(" - Client response: ");

  // Read all the lines of the reply from server 
  // and print them to Serial
  while(client.available()){
    String line = client.readStringUntil('\r');
    Serial.print(line);
  }
  
  Serial.println("");
  Serial.println(" - Closing connection");
  Serial.println("");
  
  digitalWrite(READLED, LOW);

  Serial.println("Post Data - finish");
  Serial.println("");
}


/*
 * Method to handle connecting to a given
 * wi-fi site with password
 */
void wificonnect(String ssid, String pass) {
  char idchar[32];
  char pwchar[32];

  if (ssid.length() == 0) {
    Serial.print(" - Start Wi Fi to cached entries");
    WiFi.begin();    
  } else {
    ssid.toCharArray(idchar, 32);
    pass.toCharArray(pwchar, 32);  
    Serial.print(" - Start Wi Fi to ");
    Serial.println(ssid);
    WiFi.begin(idchar, pwchar);
  }
  
  Serial.print(" - Working to connect");

  // Try to connect to Wi-Fi a prescribed number of times
  int tried = 0;
  connected = true;
  while (WiFi.status() != WL_CONNECTED) {
    digitalWrite(READLED, HIGH);
    delay(250);
    Serial.print(" .");
    digitalWrite(READLED, LOW);

    digitalWrite(ERRORLED, HIGH);
    tried = tried + 1;
    delay(250);
    digitalWrite(ERRORLED, LOW);

    if (tried >= retries) {
      connected = false;
      break;
    }    
  }
  Serial.println("");

  if (connected) {
    Serial.print(" - IP address: ");
    Serial.println(WiFi.localIP());
    Serial.print(" - Connected to ");
    if (ssid.length() == 0) { ssid = WiFi.SSID(); }
  } else {
    Serial.print(" - Failed to connect to ");
    if (ssid.length() == 0) { ssid = "cached Wi-Fi"; }
    WiFi.disconnect();
  }
  
  Serial.println(ssid);    
}


/*
 * Setup LEDs, DHT sensor, photocell, connect to wi-fi, 
 * and establish web request method
 */
void setup(void)
{
  // initialize LED pins for visual cues
  pinMode(READLED, OUTPUT);
  pinMode(ERRORLED, OUTPUT);
  pinMode(A0, INPUT);

  // You can open the Arduino IDE Serial Monitor window to see what the code is doing
  Serial.begin(115200);  // Serial connection from ESP-01 via 3.3v console cable

  Serial.println("\n\r \n\r");
  delay(500);
  
  Serial.print("Setup: Environment Watch - ID ");
  Serial.println(deviceId);
  
  Serial.println(" - Initialize temperature sensor");
  dht.begin();           // initialize temperature sensor

  // first attempt to connect to cached entries in the ESP8266
  wificonnect("", "");

  // if that didn't work, try connecting to what's in the list
  if (connected) {
    Serial.print(" - Connected to cached SSID: ");
    Serial.println(WiFi.SSID());
  } else {
    for (int i = 0; i < wifinum; i++) {
      wificonnect(ssids[i], pwds[i]);
      if (connected) { break; }
    }    
  }

  if (connected) { 
    Serial.println(" - Setting up handlers");
    server.on("/", handle_root);
    server.begin();
    Serial.println(" - HTTP server started");
  }

  // set last post to duration so we post immediately
  // once the main loop starts
  lastPost = postDuration;
  
  Serial.println("Setup complete");
  Serial.println("");
}
 

/*
 * Loop section of sketch - runs continuously & handles both
 * posting data as well as responding to web requests
 */
void loop(void)
{
  if (connected) {
    server.handleClient();

    unsigned long diff = millis() - lastPost;

    if (diff > postDuration) {
      post_data();
      lastPost = millis();
    }
  } else {
    digitalWrite(ERRORLED, HIGH);
    Serial.println(" - Could not connect!");
    delay(1000);
    digitalWrite(ERRORLED, LOW);
  }
} 

