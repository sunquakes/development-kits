#ifndef URL_H
#define URL_H

#include <QWidget>

namespace Ui {
class url;
}

class url : public QWidget
{
    Q_OBJECT

public:
    explicit url(QWidget *parent = nullptr);
    ~url();

private slots:
    void on_encodeButton_clicked();

    void on_decodeButton_clicked();

    void on_clearButton_clicked();

private:
    Ui::url *ui;
};

#endif // URL_H
