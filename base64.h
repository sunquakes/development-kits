#ifndef BASE64_H
#define BASE64_H

#include <QWidget>

namespace Ui {
class base64;
}

class base64 : public QWidget
{
    Q_OBJECT

public:
    explicit base64(QWidget *parent = nullptr);
    ~base64();

private slots:
    void on_encodeButton_clicked();

    void on_decodeButton_clicked();

    void on_clearButton_clicked();

private:
    Ui::base64 *ui;
};

#endif // BASE64_H
