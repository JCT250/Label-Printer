uint8_t initialise[2]={0x1B, 0x40};
uint8_t status_information_request[3]={0x1B, 0x69, 0x53};
uint8_t print_information_command[13]={0x1B, 0x69, 0x7A, 0xn1, 0x0A, 0xn3, 0xn4, 0xn5, 0xn6, 0xn7, 0xn8, 0xn9, 0xn10};


void setup() {
  // put your setup code here, to run once:
 Serial.begin(115200);
 
}

void loop() {
  // put your main code here, to run repeatedly: 
  
  
  Serial.write(initialise,sizeof(initialise));
  Serial.write(status_information_request,sizeof(status_information_request));
  
  
  
}
