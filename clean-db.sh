#!/bin/bash
# Database dosyalarını temizleme scripti
# Kullanım: ./clean-db.sh

echo "Database dosyaları temizleniyor..."

cd "$(dirname "$0")"

# Database dosyalarını yedekle (isteğe bağlı)
if [ -f "FitnessCenterDb.db" ]; then
    echo "Mevcut database yedekleniyor..."
    cp FitnessCenterDb.db FitnessCenterDb.db.backup 2>/dev/null || true
fi

# Database dosyalarını sil
rm -f FitnessCenterDb.db
rm -f FitnessCenterDb.db-shm
rm -f FitnessCenterDb.db-wal

echo "Database dosyaları temizlendi."
echo "Uygulamayı çalıştırdığınızda yeni database otomatik oluşturulacak."

