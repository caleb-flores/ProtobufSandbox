#!/bin/bash
MODEL_DIR=models
for f in schemas/*.proto; do
  CS_FILE=$(echo $f | awk '{split($0,a,"/"); split(a[2],b,".");print toupper(substr(b[1],0,1))substr(b[1],2)".cs"}')
  PROTO_LAST_UPDATE=$(stat -c%Y $f)

  if [ -e $MODEL_DIR/$CS_FILE ]; then
  	CS_LAST_UPDATE=$(stat -c%Y $MODEL_DIR/$CS_FILE)
  	if [ $PROTO_LAST_UPDATE -gt $CS_LAST_UPDATE ]; then
	  	echo "updating model for $f"
	  	protoc --csharp_out=$MODEL_DIR $f
	else
		echo "$CS_FILE is up to date"
  	fi 
  else
	echo "$CS_FILE does not exits"
  fi
   
  

done
