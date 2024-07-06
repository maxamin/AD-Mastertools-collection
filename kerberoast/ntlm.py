import hashlib,binascii
hash = hashlib.new('md4', "GettheUSDomain@1234".encode('utf-16le')).digest()
print binascii.hexlify(hash)