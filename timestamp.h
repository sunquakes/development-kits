#ifndef TIMESTAMP_H
#define TIMESTAMP_H

#include <QWidget>

namespace Ui {
class timestamp;
}

class timestamp : public QWidget
{
    Q_OBJECT

public:
    explicit timestamp(QWidget *parent = nullptr);
    ~timestamp();

private:
    Ui::timestamp *ui;
};

#endif // TIMESTAMP_H
