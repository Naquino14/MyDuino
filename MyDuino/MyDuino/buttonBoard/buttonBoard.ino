int bu = 2,
bd = 3,
bl = 4,
br = 5,
b1 = 6,
b2 = 7,
b3 = 8,
b4 = 9,
b5 = 10,
b6 = 11,
b7 = 12;

String signature = "gda";

void setup() {
  pinMode(bu, INPUT_PULLUP);
  pinMode(bd, INPUT_PULLUP);
  pinMode(bl, INPUT_PULLUP);
  pinMode(br, INPUT_PULLUP);
  pinMode(b1, INPUT_PULLUP);
  pinMode(b2, INPUT_PULLUP);
  pinMode(b3, INPUT_PULLUP);
  pinMode(b4, INPUT_PULLUP);
  pinMode(b5, INPUT_PULLUP);
  pinMode(b6, INPUT_PULLUP);
  pinMode(b7, INPUT_PULLUP);
  Serial.begin(9600);

}

void loop() {
  Serial.print(signature);
  r(bu);
  r(bd);
  r(bl);
  r(br);

  r(b1);
  r(b2);
  r(b3);
  r(b4);
  r(b5);
  r(b6);
  r(b7);
  Serial.println();
}

void r(int i) {
  if (digitalRead(i) == LOW) { Serial.print(1); }
  else { Serial.print(0); }
}

void p(String s) { 
  Serial.print(s); 
}
