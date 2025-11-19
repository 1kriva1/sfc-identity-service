#!/bin/bash
set -e

# === CONFIG ===
PASSWORD="Test1234!"
DAYS_CERT=730
CERTIFICATES_DIR="/mnt/c/SFC/Configurations/certificates"
ROOT_CERTIFICATE_DIR="$CERTIFICATES_DIR/root"
TARGET_DIR="$CERTIFICATES_DIR/$1"

# === 1. Create Root Folder ===
mkdir -p "$TARGET_DIR" || { echo "Failed to create $TARGET_DIR"; exit 1; }

# === 2. Create OpenSSL config for SANs ===
cat > $TARGET_DIR/san.cnf <<EOF
[ req ]
default_bits       = 2048
prompt             = no
default_md         = sha256
req_extensions     = req_ext
distinguished_name = dn

[ dn ]
CN = SERVICE_NAME

[ req_ext ]
subjectAltName = @alt_names

[ alt_names ]
DNS.1 = SERVICE_NAME
EOF

# === Function to create service cert ===
create_service_cert() {
    SERVICE=$1
    echo "Creating certificate for $SERVICE..."
    
    # Replace SERVICE_NAME in config
    sed "s/SERVICE_NAME/$SERVICE/g" $TARGET_DIR/san.cnf > $TARGET_DIR/${SERVICE}-san.cnf

    # Generate key and CSR
    openssl genrsa -out $TARGET_DIR/${SERVICE}.key 2048
    openssl req -new -key $TARGET_DIR/${SERVICE}.key -out $TARGET_DIR/${SERVICE}.csr -config $TARGET_DIR/${SERVICE}-san.cnf

    # Sign with Root CA
    openssl x509 -req -in $TARGET_DIR/${SERVICE}.csr -CA $ROOT_CERTIFICATE_DIR/sfcRootCA.crt -CAkey $ROOT_CERTIFICATE_DIR/sfcRootCA.key -CAcreateserial \
        -out $TARGET_DIR/${SERVICE}.crt -days $DAYS_CERT -sha256 -extensions req_ext -extfile $TARGET_DIR/${SERVICE}-san.cnf

    # Export to PFX
    openssl pkcs12 -export -out $TARGET_DIR/${SERVICE}.pfx -inkey $TARGET_DIR/${SERVICE}.key -in $TARGET_DIR/${SERVICE}.crt \
        -certfile $ROOT_CERTIFICATE_DIR/sfcRootCA.crt -password pass:$PASSWORD
}

# === 3. Create cert for service ===
create_service_cert $1

echo "✅ Certificates generated:"
echo " - $1.pfx (use in .NET Kestrel)"